using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DellinTerminals.Infrastructure.Data;

/// <summary>
/// Фабрика для создания DbContext в дизайн-тайме (миграции, dotnet ef)
/// Использует хардкод-коннекшен для упрощения работы с инструментами
/// </summary>
public class DellinDictionaryDbContextFactory : IDesignTimeDbContextFactory<DellinDictionaryDbContext>
{
    public DellinDictionaryDbContext CreateDbContext(string[] args)
    {
        var connectionString = 
            "Host=localhost;Port=5432;Database=dellin_dictionary;" +
            "Username=dellin_user;Password=SecureP@ss123!;" +
            "Include Error Detail=true";

        var optionsBuilder = new DbContextOptionsBuilder<DellinDictionaryDbContext>();
        optionsBuilder.UseNpgsql(
            connectionString,
            npgsql => npgsql
                .MigrationsAssembly(typeof(DellinDictionaryDbContext).Assembly.FullName)
                .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null));

        return new DellinDictionaryDbContext(optionsBuilder.Options);
    }
}