using DellinTerminals.Domain.Entities;

namespace DellinTerminals.Domain.Interfaces;

public interface ITerminalImportService
{
    Task<ImportResult> ImportAsync(string filePath, CancellationToken cancellationToken);
}