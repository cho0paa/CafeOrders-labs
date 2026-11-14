# CafeOrders

![CI](https://github.com/cho0paa/CafeOrders-lab8/actions/workflows/ci.yml/badge.svg)

Навчальний проєкт з дисципліни «Конструювання програмного
забезпечення». Варіант № 12: замовлення та меню кав’ярні.

## Склад рішення

- CafeOrders.Core — логіка предметної області (бібліотека класів)
- CafeOrders.App — консольний застосунок (введення, виведення)

## Вимоги

.NET SDK 8.0 (версію зафіксовано у `global.json`), Git.

## Складання та запуск

    dotnet build
    dotnet run --project CafeOrders.App

## Конвеєр безперервної інтеграції

Файл `.github/workflows/ci.yml` на кожен push і pull request виконує: відновлення залежностей,
`dotnet format --verify-no-changes`, складання Release з аналізаторами (попередження — помилки),
тести з покриттям і порогом 60 % рядків. Локальний еквівалент:

    dotnet format --verify-no-changes
    dotnet build -c Release
    dotnet test -c Release /p:CollectCoverage=true /p:Threshold=60 /p:ThresholdType=line

## Стандарт кодування

Правила описано у файлі CODING_STANDARD.md і закріплено
в .editorconfig. Перевірка: dotnet format --verify-no-changes

## Автор

Новіков Артем, група 471
