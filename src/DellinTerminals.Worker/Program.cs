using DellinTerminals.Application;
using DellinTerminals.Infrastructure;
using DellinTerminals.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<TerminalsImportBackgroundService>();

var host = builder.Build();

// Применение миграций при старте (опционально, для удобства разработки)
// using (var scope = host.Services.CreateScope())
// {
//     var context = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
//     await context.Database.MigrateAsync();
// }

await host.RunAsync();