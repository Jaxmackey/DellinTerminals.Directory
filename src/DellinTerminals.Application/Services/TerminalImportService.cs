using System.Text.Json;
using System.Text.Json.Serialization;
using DellinTerminals.Domain.Entities;
using DellinTerminals.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DellinTerminals.Application.Services;

public class TerminalImportService : ITerminalImportService
{
    private readonly IOfficeRepository _repository;
    private readonly ILogger<TerminalImportService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public TerminalImportService(
        IOfficeRepository repository,
        ILogger<TerminalImportService> logger)
    {
        _repository = repository;
        _logger = logger;
        
        // Настройки десериализации: case-insensitive + ignore nulls
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };
    }

    public async Task<ImportResult> ImportAsync(
        string filePath, 
        CancellationToken cancellationToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            // 1. Загрузка и десериализация JSON
            _logger.LogInformation("Starting import from {FilePath}", filePath);
            
            if (!File.Exists(filePath))
            {
                _logger.LogError("File not found: {FilePath}", filePath);
                return new ImportResult 
                { 
                    Success = false, 
                    ErrorMessage = $"File not found: {filePath}" 
                };
            }

            var jsonContent = await File.ReadAllTextAsync(filePath, cancellationToken);
            var jsonTerminals = JsonSerializer.Deserialize<IEnumerable<JsonTerminal>>(jsonContent, _jsonOptions);
            
            if (jsonTerminals == null || !jsonTerminals.Any())
            {
                _logger.LogWarning("No terminals found in JSON file");
                return new ImportResult { LoadedCount = 0, DeletedCount = 0, InsertedCount = 0, Duration = stopwatch.Elapsed, Success = true };
            }

            var loadedCount = jsonTerminals.Count();
            _logger.LogInformation("Загружено {Count} терминалов из JSON", loadedCount);

            // 2. Очистка существующих данных
            var oldCount = await _repository.CountAsync(cancellationToken);
            var deletedCount = await _repository.DeleteAllAsync(cancellationToken);
            _logger.LogInformation("Удалено {OldCount} старых записей", deletedCount);

            // 3. Маппинг и вставка новых данных
            var offices = MapJsonToDomain(jsonTerminals);
            await _repository.BulkInsertAsync(offices, cancellationToken);
            
            var insertedCount = offices.Count();
            _logger.LogInformation("Сохранено {NewCount} новых терминалов", insertedCount);

            stopwatch.Stop();
            _logger.LogInformation("Import completed in {ElapsedSeconds:F2} seconds", stopwatch.Elapsed.TotalSeconds);

            return new ImportResult
            {
                LoadedCount = loadedCount,
                DeletedCount = deletedCount,
                InsertedCount = insertedCount,
                Duration = stopwatch.Elapsed,
                Success = true
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Import cancelled by graceful shutdown");
            return new ImportResult { Success = false, ErrorMessage = "Operation cancelled" };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Ошибка импорта: {Exception}", ex.Message);
            return new ImportResult 
            { 
                Success = false, 
                ErrorMessage = ex.Message,
                Duration = stopwatch.Elapsed
            };
        }
    }

    private IEnumerable<Office> MapJsonToDomain(IEnumerable<JsonTerminal> jsonTerminals)
    {
        foreach (var json in jsonTerminals)
        {
            var office = new Office
            {
                Code = json.Code,
                CityCode = json.CityCode,
                Uuid = json.Uuid,
                Type = json.Type,
                CountryCode = json.CountryCode, 
                Coordinates = json.Coordinates != null ? new Coordinates(json.Coordinates.Latitude, 
                    json.Coordinates.Longitude) : null, 
                AddressRegion = json.AddressRegion,
                AddressCity = json.AddressCity,
                AddressStreet = json.AddressStreet,
                AddressHouseNumber = json.AddressHouseNumber,
                AddressApartment = json.AddressApartment,
                WorkTime = json.WorkTime,
                NormalizedCityName = (json.AddressCity ?? string.Empty).Trim().ToLowerInvariant(),
                Phones = json.Phones?.Select(p => new Phone
                {
                    PhoneNumber = p.PhoneNumber,
                    Additional = p.Additional
                }).ToList() ?? new List<Phone>()
            };
            
            yield return office;
        }
    }
}