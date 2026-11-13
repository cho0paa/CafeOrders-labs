# Стандарт кодування CafeOrders

Початкову версію правил (ЛР1) замінено стандартом 1.0.0:

- [docs/standard/STANDARD.md](docs/standard/STANDARD.md) — правила з рівнями обов’язковості;
- [docs/standard/REVIEW_CHECKLIST.md](docs/standard/REVIEW_CHECKLIST.md) — чекліст огляду коду;
- [docs/standard/DECISIONS.md](docs/standard/DECISIONS.md) — журнал рішень щодо стандарту.

Машинна частина: `.editorconfig`, `Directory.Build.props`, `CodeMetricsConfig.txt`.
Перевірка перед комітом:

    dotnet format --verify-no-changes
    dotnet build -warnaserror
