using DellinTerminals.Domain.Interfaces;
using DellinTerminals.Infrastructure.Data;
using DellinTerminals.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DellinTerminals.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DellinDictionaryDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"),
                npgsql => npgsql
                    .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)
                    .MigrationsAssembly(typeof(DellinDictionaryDbContext).Assembly.FullName));
            
            // Для логирования запросов в Development
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
#endif
        });

        services.AddScoped<IDbContext>(sp => sp.GetRequiredService<DellinDictionaryDbContext>());
        services.AddScoped<IOfficeRepository, OfficeRepository>();

        return services;
    }
}