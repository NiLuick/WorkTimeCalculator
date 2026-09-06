using System.IO;
using System.Text.Json;

using WorkTimeCalculator.DTO;

namespace WorkTimeCalculator.Service;

public class WorkTimeCalculationService
{
    private readonly TimeDTO _settings;

    public WorkTimeCalculationService()
    {
        _settings = LoadSettings();
    }

    private static TimeDTO LoadSettings(string fileName = "appsettings.json")
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);

        if (!File.Exists(path))
            return new TimeDTO { WorkTime = "7:00" };

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<TimeDTO>(json) ?? new TimeDTO { WorkTime = "7:00" };
    }

    private TimeSpan GetTotalBreakDuration()
    {
        var lunch = TimeSpan.Parse(_settings.Lunch.End) - TimeSpan.Parse(_settings.Lunch.Start);
        var pause = TimeSpan.Parse(_settings.Break.End) - TimeSpan.Parse(_settings.Break.Start);
        return lunch + pause;
    }
    
    public TimeSpan CalculateOvertime(TimeSpan start, TimeSpan end)
    {
        var worked = end - start - GetTotalBreakDuration();
        var workTime = TimeSpan.Parse(_settings.WorkTime);
        return worked - workTime;
    }
    
    public TimeSpan CalculateGoHomeTime(TimeSpan start)
    {
        var workTime = TimeSpan.Parse(_settings.WorkTime);
        return start + workTime + GetTotalBreakDuration();
    }
}