using DellinTerminals.Application.Common.Behaviors;
using DellinTerminals.Application.Services;
using DellinTerminals.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DellinTerminals.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
        
        services.AddSingleton<IScheduleService, CronScheduleService>();
        services.AddScoped<ITerminalImportService, TerminalImportService>();
        return services;
    }
}