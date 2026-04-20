# 🚚 DellinTerminals.Directory

> Справочник терминалов доставки с фоновой синхронизацией из JSON и REST API.  
> Реализация тестового задания: фоновая загрузка данных, поиск терминалов, DDD + CQRS архитектура.

---

## ℹ️ О проекте

**DellinTerminals.Directory** — это сервис для управления справочником терминалов доставки. Приложение периодически загружает данные из JSON-файла, сохраняет их в PostgreSQL и предоставляет REST API для поиска терминалов по городу и региону.

### ✨ Ключевые возможности

| Функция | Описание |
|---------|----------|
| 🔄 Фоновая синхронизация | Ежедневная загрузка данных в 02:00 MSK (настраивается) |
| 🔍 Поиск терминалов | По названию города и области |
| 🆔 Получение CityId | Возврат идентификатора города для интеграций |
| 🗄️ Эффективное хранение | Индексы для быстрого поиска, оптимизированные запросы |
| 🪵 Структурированное логирование | Все операции логируются в консоль в машиночитаемом формате |
| 🛡️ Надёжность | Обработка ошибок без падения сервиса, graceful shutdown |

---

## 🛠️ Технический стек

| Компонент | Технология |
|-----------|-----------|
| **Язык** | C# 13 (.NET 9) |
| **Платформа** | ASP.NET Core 9, Generic Host |
| **База данных** | PostgreSQL 16 + Npgsql EF Core Provider |
| **ORM** | Entity Framework Core 9 (Code First, миграции) |
| **Архитектура** | Clean Architecture + DDD + CQRS |
| **Медиатор** | MediatR 12 (CQRS-запросы, пайплайн-поведения) |
| **Логирование** | Microsoft.Extensions.Logging (структурированные логи) |
| **Сериализация** | System.Text.Json (case-insensitive) |
| **Контейнеризация** | Docker + Docker Compose (PostgreSQL) |
| **DI** | Microsoft.Extensions.DependencyInjection |

> ✅ **Без сторонних библиотек** (кроме минимально необходимых: MediatR, Npgsql)

---

## 🏗️ Архитектура

Проект реализует **слоистую архитектуру** с чётким разделением ответственности:

```
┌─────────────────────────────────────────┐
│              API Layer                   │
│  • Controllers (TerminalsController)     │
│  • Contracts (DTO для клиентов)          │
│  • Mappers (Domain → API)                │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│           Application Layer             │
│  • CQRS Queries (FindOfficesByCity)     │
│  • Handlers + Validators                │
│  • Services (TerminalImportService)     │
│  • Pipeline Behaviors (Logging)         │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│             Domain Layer                │
│  • Entities (Office, Phone)             │
│  • Value Objects (Coordinates)          │
│  • Enums (OfficeType)                   │
│  • Interfaces (IOfficeRepository)       │
│  • Domain Exceptions                    │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│         Infrastructure Layer            │
│  • EF Core DbContext + Configurations   │
│  • Repositories (OfficeRepository)      │
│  • JSON Models (для парсинга)           │
│  • Mappers (Json ↔ Domain, Entity ↔ Domain)│
│  • External Services (если будут)       │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│           Worker / Host                 │
│  • BackgroundService (синхронизация)    │
│  • Schedule Service (cron-проверка)     │
│  • Composition Root (Program.cs)        │
└─────────────────────────────────────────┘
```

### 🔑 Принципы

1. **Dependency Rule**: Зависимости направлены внутрь → Domain не знает о внешних слоях.
2. **CQRS**: Разделение команд и запросов, каждый хендлер отвечает за одну операцию.
3. **Mediator Pattern**: MediatR для декуплинга отправителей и обработчиков.
4. **Маппинг между слоями**: Явные преобразователи (не используем автоматические мапперы).
5. **Граничные контракты**: API возвращает только `Contracts`, не доменные модели.

---

## 📁 Структура проекта

```
DellinTerminals.Directory/
├── src/
│   ├── DellinTerminals.Domain/          # Чистый домен (без зависимостей)
│   │   ├── Entities/ (Office, Phone)
│   │   ├── ValueObjects/ (Coordinates)
│   │   ├── Enums/ (OfficeType)
│   │   ├── Interfaces/ (IOfficeRepository)
│   │   └── Exceptions/
│   │
│   ├── DellinTerminals.Application/     # Бизнес-логика
│   │   ├── Terminals/
│   │   │   ├── Queries/ (CQRS-запросы)
│   │   │   └── Commands/ (если будут)
│   │   ├── Services/ (TerminalImportService)
│   │   ├── Common/Behaviors/ (LoggingBehavior)
│   │   └── ApplicationServiceCollectionExtensions.cs
│   │
│   ├── DellinTerminals.Infrastructure/  # Внешняя инфраструктура
│   │   ├── Data/
│   │   │   ├── Entities/ (OfficeEntity, PhoneEntity)
│   │   │   ├── Configurations/ (EF Core конфигурации)
│   │   │   ├── DellinDictionaryDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Json/ (модели для парсинга JSON)
│   │   ├── Mappers/ (DomainMapper: Json↔Domain, Entity↔Domain)
│   │   ├── Repositories/ (OfficeRepository)
│   │   └── InfrastructureServiceCollectionExtensions.cs
│   │
│   ├── DellinTerminals.Api/             # REST API
│   │   ├── Controllers/ (TerminalsController)
│   │   ├── Contracts/ (DTO для клиентов)
│   │   │   └── Terminals/ (OfficeResponse, CityIdResponse)
│   │   ├── Mappers/ (DomainToApiMapper)
│   │   ├── Program.cs
│   │   └── appsettings*.json
│   │
│   └── DellinTerminals.Worker/          # Фоновая служба
│       ├── Services/ (CronScheduleService)
│       ├── TerminalsImportBackgroundService.cs
│       ├── Program.cs
│       └── appsettings*.json
│
├── files/
│   └── terminals.json                   # Источник данных
│
├── docker-compose.yml                   # PostgreSQL контейнер
├── .gitignore
└── README.md
```

---

## 🚀 Быстрый старт

### Предварительные требования

- .NET 9 SDK: https://dotnet.microsoft.com/download
- Docker Desktop: https://www.docker.com/products/docker-desktop
- PowerShell 7+ (рекомендуется)

### 1. Запуск базы данных

```bash
# Из корня репозитория
docker-compose up -d

# Проверка готовности
docker exec dellin_dictionary_db pg_isready -U dellin_user -d dellin_dictionary
# Ожидаемый ответ: "accepting connections"
```

### 2. Применение миграций

```bash
# Создание и применение миграций
dotnet ef migrations add InitialCreate `
  -p src/DellinTerminals.Infrastructure `
  -s src/DellinTerminals.Worker `
  -c DellinDictionaryDbContext `
  -o Data/Migrations

dotnet ef database update `
  -p src/DellinTerminals.Infrastructure `
  -s src/DellinTerminals.Worker `
  -c DellinDictionaryDbContext
```

> 💡 Если `dotnet-ef` не установлен:  
> `dotnet tool install --global dotnet-ef`

### 3. Запуск приложения

#### Вариант А: Фоновая служба (импорт данных)

```bash
cd src/DellinTerminals.Worker
dotnet run
```

#### Вариант Б: REST API

```bash
cd src/DellinTerminals.Api
dotnet run
```

#### Вариант В: Оба одновременно (в разных терминалах)

### 4. Проверка работы

```bash
# Swagger UI (если включён)
open http://localhost:5130/swagger

# Тестовые запросы
curl "http://localhost:5130/api/terminals/offices?city=Москва"
curl "http://localhost:5130/api/terminals/offices?city=Санкт-Петербург&region=Ленинградская"
curl "http://localhost:5130/api/terminals/city-id?city=Москва"
```

---

## 🌐 API

### Базовый путь: `/api/terminals`

#### 🔍 Поиск офисов по городу

```
GET /api/terminals/offices?city={cityName}&region={regionName?}
```

**Параметры:**

| Параметр | Тип | Обязательный | Описание |
|----------|-----|--------------|----------|
| `city` | string | ✅ | Название города (регистронезависимый поиск) |
| `region` | string | ❌ | Название области/региона для уточнения |

**Ответ (200 OK):**
```json
[
  {
    "id": 1,
    "code": "MOW001",
    "cityCode": 495,
    "uuid": "550e8400-e29b-41d4-a716-446655440001",
    "type": "PVZ",
    "countryCode": "RU",
    "coordinates": {
      "latitude": 55.751244,
      "longitude": 37.618423
    },
    "addressRegion": "Москва",
    "addressCity": "Москва",
    "addressStreet": "Тверская",
    "addressHouseNumber": "15",
    "addressApartment": null,
    "workTime": "09:00-21:00",
    "phones": [
      {
        "phoneNumber": "+7 (495) 123-45-67",
        "additional": "доб. 101"
      }
    ]
  }
]
```

**Коды ответов:**

| Код | Описание |
|-----|----------|
| `200` | Успешный поиск, возвращён список офисов |
| `400` | Не указан обязательный параметр `city` |
| `500` | Внутренняя ошибка сервера |

---

#### 🆔 Получение идентификатора города

```
GET /api/terminals/city-id?city={cityName}&region={regionName?}
```

**Параметры:** аналогично поиску офисов.

**Ответ (200 OK):**
```json
{ "cityId": 495 }
```

**Коды ответов:**

| Код | Описание |
|-----|----------|
| `200` | Город найден, возвращён `cityId` |
| `400` | Не указан обязательный параметр `city` |
| `404` | Город не найден в справочнике |
| `500` | Внутренняя ошибка сервера |

---

## ⚙️ Конфигурация

### Переменные окружения / appsettings

| Ключ | Значение по умолчанию | Описание |
|------|----------------------|----------|
| `ConnectionStrings:Default` | `Host=localhost;Port=5432;Database=dellin_dictionary;Username=dellin_user;Password=SecureP@ss123!` | Строка подключения к PostgreSQL |
| `Files:TerminalsFileName` | `terminals.json` | Имя файла с терминалами (ищется в папке `files/` относительно `ContentRootPath`) |
| `ImportSchedule:CronExpression` | `0 2 * * *` | Cron-выражение для запуска импорта (по умолчанию — ежедневно в 02:00) |
| `ImportSchedule:TimeZoneId` | `Russian Standard Time` | Часовой пояс для расписания (MSK) |
| `ImportSchedule:RunOnStartup` | `true` (только Development) | Запускать импорт сразу при старте в режиме разработки |
| `Logging:LogLevel:Default` | `Information` | Минимальный уровень логирования |

### Пример `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=dellin_dictionary;Username=dellin_user;Password=SecureP@ss123!;Include Error Detail=true"
  },
  "Files": {
    "TerminalsFileName": "terminals.json"
  },
  "ImportSchedule": {
    "CronExpression": "*/5 * * * *",
    "TimeZoneId": "Russian Standard Time",
    "RunOnStartup": true
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Warning",
      "DellinTerminals": "Debug"
    },
    "Console": {
      "FormatterName": "simple",
      "FormatterOptions": {
        "TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff "
      }
    }
  }
}
```

---

## 🪵 Логирование

Приложение использует структурированное логирование через `ILogger<T>`. Формат вывода настраивается через `appsettings*.json`.

### Примеры логов

```
2026-04-20 14:30:00.123 [INF] Background service started
2026-04-20 14:30:00.145 [INF] Terminals file: C:\...\files\terminals.json
2026-04-20 14:30:00.156 [INF] Next scheduled run: 4/21/2026 02:00:00 +03:00
2026-04-20 14:30:00.167 [INF] Development mode: forcing import on startup
2026-04-20 14:30:00.178 [INF] Starting import from ...\files\terminals.json
2026-04-20 14:30:00.189 [INF] Загружено 2 терминалов из JSON
2026-04-20 14:30:00.350 [INF] Удалено 0 старых записей
2026-04-20 14:30:00.500 [INF] Сохранено 2 новых терминалов
2026-04-20 14:30:00.510 [INF] Import successful: 2 loaded, 0 deleted, 2 inserted in 0.32s
```

### Обработка ошибок

- Ошибки импорта логируются с уровнем `Error`, но **не останавливают** фоновую службу.
- При `OperationCanceledException` (graceful shutdown) сервис корректно завершает работу.
- Все исключения в цикле `ExecuteAsync` перехватываются, логируются, и сервис продолжает работу после паузы.

---

## 🧪 Тестирование

### Ручное тестирование

1. Убедитесь, что БД запущена: `docker ps | Select-String dellin`
2. Запустите Worker для импорта данных: `dotnet run` в папке `Worker`
3. Запустите API: `dotnet run` в папке `Api`
4. Выполните запросы через curl, Postman или Swagger

### Проверка данных в БД

```bash
# Подключиться к PostgreSQL
docker exec -it dellin_dictionary_db psql -U dellin_user -d dellin_dictionary

# Посмотреть данные
SELECT id, code, address_city, latitude, longitude FROM offices;
SELECT * FROM phones LIMIT 5;

# Проверить индексы
SELECT indexname, indexdef FROM pg_indexes WHERE tablename = 'offices';
```

Для запуска:
```bash
dotnet test tests/DellinTerminals.Tests.Unit
dotnet test tests/DellinTerminals.Tests.Integration
```
