using DellinTerminals.Domain.Entities;

namespace DellinTerminals.Domain.Interfaces;

public interface IOfficeRepository
{
    Task<IEnumerable<Office>> GetOfficesByCityAsync(string cityName, string? regionName, CancellationToken ct);
    Task<int?> GetCityIdByOfficeAsync(string cityName, string? regionName, CancellationToken ct);
    Task BulkInsertAsync(IEnumerable<Office> offices, CancellationToken ct);
    Task<int> DeleteAllAsync(CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct); 
}