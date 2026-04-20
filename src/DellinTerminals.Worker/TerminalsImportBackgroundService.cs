using DellinTerminals.Domain.Interfaces;

namespace DellinTerminals.Worker;

public class TerminalsImportBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IScheduleService _scheduleService;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<TerminalsImportBackgroundService> _logger;
    
    private readonly string _terminalsFileName; 
    private readonly string _cronExpression;
    private readonly string _timeZoneId;
    private readonly bool _runOnStartup;
    
    private TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public TerminalsImportBackgroundService(
        IServiceScopeFactory scopeFactory,
        IScheduleService scheduleService,
        IConfiguration configuration,
        IHostEnvironment environment, 
        ILogger<TerminalsImportBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _scheduleService = scheduleService;
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
        
        _terminalsFileName = _configuration.GetValue<string>("Files:TerminalsFileName") 
            ?? "terminals.json";
        
        _cronExpression = _configuration.GetValue<string>("ImportSchedule:CronExpression") 
            ?? "0 2 * * *";
        _timeZoneId = _configuration.GetValue<string>("ImportSchedule:TimeZoneId") 
            ?? "Russian Standard Time";
        _runOnStartup = _configuration.GetValue<bool>("ImportSchedule:RunOnStartup");
    }
    
    private string GetTerminalsFilePath()
    {
        var contentRoot = _environment.ContentRootPath;

        var filesDir = Path.Combine(contentRoot, "files");
        
        if (!Directory.Exists(filesDir))
        {
            var parent = Directory.GetParent(contentRoot)?.Parent;
            if (parent != null)
            {
                filesDir = Path.Combine(parent.FullName, "files");
            }
        }
        
        var fullPath = Path.Combine(filesDir, _terminalsFileName);
        
        _logger.LogDebug("Resolved terminals path: {FullPath} (exists: {Exists})", 
            fullPath, File.Exists(fullPath));
        
        return fullPath;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var terminalsPath = GetTerminalsFilePath();
        
        _logger.LogInformation("Background service started");
        _logger.LogInformation("Terminals file: {Path}", terminalsPath);
        _logger.LogInformation("Next scheduled run: {NextRun}", 
            _scheduleService.GetNextRunTime(_cronExpression, _timeZoneId));

        // Проверка файла при старте (только лог, не ошибка)
        if (!File.Exists(terminalsPath))
        {
            _logger.LogWarning("Terminals file not found at startup: {Path}", terminalsPath);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTimeOffset.Now;
                
                if (_scheduleService.IsTimeToRun(now, _cronExpression, _timeZoneId, _runOnStartup))
                {
                    var importPath = GetTerminalsFilePath();
                    
                    if (!File.Exists(importPath))
                    {
                        _logger.LogError("Cannot import: file not found: {Path}", importPath);
                        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                        continue;
                    }
                    
                    _logger.LogInformation("Schedule triggered. Starting import from {Path}", importPath);
                    
                    await using var scope = _scopeFactory.CreateAsyncScope();
                    var importService = scope.ServiceProvider.GetRequiredService<ITerminalImportService>();
                    
                    var result = await importService.ImportAsync(importPath, stoppingToken);
                    
                    if (result.Success)
                    {
                        _logger.LogInformation("Import successful: {Loaded} loaded, {Deleted} deleted, {Inserted} inserted in {Duration:F2}s", 
                            result.LoadedCount, result.DeletedCount, result.InsertedCount, result.Duration.TotalSeconds);
                    }
                    else
                    {
                        _logger.LogError("Import failed: {Error}", result.ErrorMessage);
                    }
                    
                    await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
                }
                
                if (stoppingToken.IsCancellationRequested) break;
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background service stopping gracefully...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in background service loop");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
        
        _logger.LogInformation("Background service stopped");
    }
}