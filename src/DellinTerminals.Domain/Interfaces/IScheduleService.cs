namespace DellinTerminals.Domain.Interfaces;

public interface IScheduleService
{
    bool IsTimeToRun(DateTimeOffset currentTime, string cronExpression, string timeZoneId, bool runOnStartup = false);
    DateTimeOffset GetNextRunTime(string cronExpression, string timeZoneId);
}