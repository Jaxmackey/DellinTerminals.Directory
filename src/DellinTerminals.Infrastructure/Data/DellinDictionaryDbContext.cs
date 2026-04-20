using DellinTerminals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminals.Infrastructure.Data;

public class DellinDictionaryDbContext(DbContextOptions<DellinDictionaryDbContext> options)
    : DbContext(options), IDbContext
{
    public DbSet<Office> Offices => Set<Office>();
    public DbSet<Phone> Phones => Set<Phone>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DellinDictionaryDbContext).Assembly);
    }
}