namespace DellinTerminals.Domain.Entities;

public class ImportResult
{
    public int LoadedCount { get; init; }
    public int DeletedCount { get; init; }
    public int InsertedCount { get; init; }
    public TimeSpan Duration { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}