using DellinTerminals.Application;
using DellinTerminals.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 2. Сервисы
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 3. Swagger для тестов
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

var app = builder.Build();

// Swagger только в Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dellin Terminals API v1"));
}

app.MapControllers();
app.Run();
