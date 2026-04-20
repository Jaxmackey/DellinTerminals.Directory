using DellinTerminals.Application;
using DellinTerminals.Infrastructure;
using DellinTerminals.Worker;

var builder = Host.CreateApplicationBuilder(args);

// 1. Конфигурация
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2. Логирование (структурированное, как в ТЗ)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// 3. Слои приложения
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 4. Регистрация фоновой службы
builder.Services.AddHostedService<TerminalsImportBackgroundService>();

// 5. Регистрация API-контроллеров (если используешь Api проект)
// builder.Services.AddControllers();

var host = builder.Build();

// Применение миграций при старте (опционально, для удобства разработки)
// using (var scope = host.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
//     await context.Database.MigrateAsync();
// }

await host.RunAsync();