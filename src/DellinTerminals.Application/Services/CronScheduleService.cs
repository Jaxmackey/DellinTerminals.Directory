using DellinTerminals.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DellinTerminals.Application.Services;

public class CronScheduleService : IScheduleService
{
    private readonly ILogger<CronScheduleService> _logger;
    private bool _isFirstCheck = true;

    public CronScheduleService(ILogger<CronScheduleService> logger)
    {
        _logger = logger;
    }

    public bool IsTimeToRun(DateTimeOffset currentTime, string cronExpression, string timeZoneId, bool runOnStartup)
    {
        // Для разработки: если флаг включён и это первый запуск — разрешаем выполнение
        if (runOnStartup && _isFirstCheck)
        {
            _isFirstCheck = false;
            _logger.LogInformation("🧪 Development mode: forcing import on startup");
            return true;
        }
        
        // Поддерживаем упрощённый формат: "0 2 * * *" (минута час день месяц день-недели)
        // Для 02:00 каждый день
        var parts = cronExpression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5)
        {
            _logger.LogWarning("Invalid cron expression: {Cron}", cronExpression);
            return false;
        }

        // Конвертируем время в целевой часовой пояс
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var targetTime = TimeZoneInfo.ConvertTime(currentTime, timeZone);

        var minute = int.Parse(parts[0]);
        var hour = int.Parse(parts[1]);
        // parts[2-4] = *, *, * → каждый день

        // Проверяем точное совпадение минуты и часа (с допуском 1 минута)
        return targetTime.Hour == hour && Math.Abs(targetTime.Minute - minute) <= 1;
    }

    public DateTimeOffset GetNextRunTime(string cronExpression, string timeZoneId)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        var now = DateTimeOffset.Now;
        var targetTime = TimeZoneInfo.ConvertTime(now, timeZone);
        
        // Если сегодня уже прошло 02:00 — следующее выполнение завтра
        if (targetTime.Hour >= 2)
        {
            targetTime = targetTime.AddDays(1);
        }
        
        // Устанавливаем время 02:00:00
        targetTime = new DateTimeOffset(
            targetTime.Year, targetTime.Month, targetTime.Day, 
            2, 0, 0, targetTime.Offset);
        
        // Конвертируем обратно в локальное время для сравнения
        return TimeZoneInfo.ConvertTime(targetTime, TimeZoneInfo.Local);
    }
}