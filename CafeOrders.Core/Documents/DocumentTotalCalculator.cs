using CafeOrders.Core.Domain;

namespace CafeOrders.Core.Documents;

/// <summary>Розрахунок підсумку замовлення кав’ярні як послідовність простих кроків.</summary>
public static class DocumentTotalCalculator
{
    /// <summary>Обчислює суму рядків зі знижкою за обсягом рядка. Не змінює рядки.</summary>
    /// <param name="lines">Рядки замовлення, щонайменше один.</param>
    /// <returns>Сума рядків до знижок замовлення, грн.</returns>
    public static decimal SubtotalOf(IReadOnlyList<OrderLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        DocumentGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (OrderLine line in lines)
        {
            decimal sum = line.Quantity * line.UnitPrice;
            if (line.Quantity >= PricingRules.BulkQuantity)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Обчислює знижку постійного гостя.</summary>
    /// <param name="amount">Сума, від якої рахують знижку, невід’ємна.</param>
    /// <param name="isRegularClient">Чи є гість постійним.</param>
    /// <returns>Сума знижки, грн.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularClient)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return isRegularClient ? amount * PricingRules.RegularClientRate : 0m;
    }

    /// <summary>Обчислює знижку за обсягом замовлення.</summary>
    /// <param name="amount">Сума замовлення, невід’ємна.</param>
    /// <returns>Сума знижки, грн.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        if (amount > PricingRules.LargeOrderFrom)
        {
            return amount * PricingRules.LargeOrderRate;
        }

        if (amount > PricingRules.MediumOrderFrom)
        {
            return amount * PricingRules.MediumOrderRate;
        }

        return 0m;
    }

    /// <summary>Обчислює знижку за кодом купона.</summary>
    /// <param name="amount">Сума замовлення, невід’ємна.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Сума знижки, грн; 0 для невідомого купона.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Обчислює знижку за день створення замовлення.</summary>
    /// <param name="amount">Сума замовлення, невід’ємна.</param>
    /// <param name="createdAt">Дата створення замовлення.</param>
    /// <returns>Сума знижки, грн; ненульова лише в неділю.</returns>
    public static decimal WeekendDiscountOf(decimal amount, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday ? amount * PricingRules.WeekendRate : 0m;
    }

    /// <summary>Обчислює плату за доставку замовлення.</summary>
    /// <param name="amount">Сума замовлення після знижок.</param>
    /// <param name="deliveryPrice">Тариф доставки, невід’ємний.</param>
    /// <returns>Плата за доставку, грн; 0 для замовлення від 1000 грн.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom ? deliveryPrice : 0m;
    }

    /// <summary>Визначає наступний стан замовлення. Не змінює жодного об’єкта.</summary>
    /// <param name="current">Поточний стан.</param>
    /// <param name="total">Підсумок замовлення.</param>
    /// <returns>Наступний стан або поточний, якщо перехід не передбачено.</returns>
    public static OrderStatus NextStatusOf(OrderStatus current, decimal total)
        => current switch
        {
            OrderStatus.New when total > 0m => OrderStatus.Paid,
            OrderStatus.Paid => OrderStatus.Served,
            _ => current,
        };

    /// <summary>
    /// Розраховує підсумок замовлення та наступний стан. Запит без побічних ефектів.
    /// </summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <returns>Підсумок, округлений до копійок, і наступний стан.</returns>
    public static TotalResult Calculate(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        DocumentGuards.EnsureRequestValid(request);
        decimal total = SubtotalOf(request.Lines);
        total -= ClientDiscountOf(total, request.Client.IsRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, request.CouponCode);
        total -= WeekendDiscountOf(total, request.CreatedAt);
        total += DeliveryFeeOf(total, request.DeliveryPrice);
        total = request.Status == OrderStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        return new TotalResult(total, NextStatusOf(request.Status, total));
    }
}
