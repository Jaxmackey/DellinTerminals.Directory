using DellinTerminals.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminals.Infrastructure.Data;

public interface IDbContext
{
    DbSet<OfficeEntity> Offices { get; }
    DbSet<PhoneEntity> Phones { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}