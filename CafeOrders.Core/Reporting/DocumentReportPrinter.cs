namespace CafeOrders.Core.Reporting;

/// <summary>Друкує готовий текст чека в переданий потік. Команда: нічого не повертає.</summary>
public sealed class DocumentReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Ініціалізує принтер потоком виводу.</summary>
    /// <param name="output">Куди писати: консоль, файл або StringWriter у тестах.</param>
    public DocumentReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує текст чека в потік.</summary>
    /// <param name="report">Непорожній текст чека.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
