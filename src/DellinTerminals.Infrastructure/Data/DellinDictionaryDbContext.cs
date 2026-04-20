using DellinTerminals.Domain.Entities;
using DellinTerminals.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminals.Infrastructure.Data;

public class DellinDictionaryDbContext(DbContextOptions<DellinDictionaryDbContext> options)
    : DbContext(options), IDbContext
{
    public DbSet<OfficeEntity> Offices => Set<OfficeEntity>();
    public DbSet<PhoneEntity> Phones => Set<PhoneEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DellinDictionaryDbContext).Assembly);
    }
}