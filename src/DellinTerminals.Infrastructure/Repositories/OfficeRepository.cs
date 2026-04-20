using Microsoft.EntityFrameworkCore;
using DellinTerminals.Domain.Entities;
using DellinTerminals.Domain.Interfaces;
using DellinTerminals.Infrastructure.Data;
using DellinTerminals.Infrastructure.Mappers;

namespace DellinTerminals.Infrastructure.Repositories;

public class OfficeRepository : IOfficeRepository
{
    private readonly DellinDictionaryDbContext _context;

    public OfficeRepository(DellinDictionaryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Office>> GetOfficesByCityAsync(
        string cityName, string? regionName, CancellationToken ct)
    {
        var normalized = cityName.Trim().ToLowerInvariant();
        
        var query = _context.Offices
            .AsNoTracking()
            .Include(o => o.Phones)
            .Where(o => o.NormalizedCityName == normalized);
        
        if (!string.IsNullOrWhiteSpace(regionName))
        {
            var normalizedRegion = regionName.Trim().ToLowerInvariant();
            query = query.Where(o => 
                EF.Functions.ILike(o.AddressRegion, $"%{normalizedRegion}%"));
        }
        
        var entities = await query.ToListAsync(ct);
        return entities.Select(e => e.ToDomain());
    }

    public async Task<int?> GetCityIdByOfficeAsync(
        string cityName, string? regionName, CancellationToken ct)
    {
        var normalized = cityName.Trim().ToLowerInvariant();
        
        var query = _context.Offices
            .AsNoTracking()
            .Where(o => o.NormalizedCityName == normalized);
        
        if (!string.IsNullOrWhiteSpace(regionName))
        {
            var normalizedRegion = regionName.Trim().ToLowerInvariant();
            query = query.Where(o => 
                EF.Functions.ILike(o.AddressRegion, $"%{normalizedRegion}%"));
        }
        
        return await query.Select(o => (int?)o.CityCode).FirstOrDefaultAsync(ct);
    }

    public async Task BulkInsertAsync(IEnumerable<Office> offices, CancellationToken ct)
    {
        var entities = offices.Select(o => o.ToEntity());
        await _context.Offices.AddRangeAsync(entities, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<int> DeleteAllAsync(CancellationToken ct)
    {
        return await _context.Offices.ExecuteDeleteAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct)
    {
        return await _context.Offices.CountAsync(ct);
    }
}