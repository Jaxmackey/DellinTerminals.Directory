using DellinTerminals.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DellinTerminals.Infrastructure.Data;

public interface IDbContext
{
    DbSet<Office> Offices { get; }
    DbSet<Phone> Phones { get; }
    Task<int> SaveChangesAsync(CancellationToken ct);
}