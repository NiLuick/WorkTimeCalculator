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
 
    // Returns the portion of [rangeStart, rangeEnd] that overlaps with [otherStart, otherEnd].
    // Returns TimeSpan.Zero if the two ranges don't overlap at all.
    private static TimeSpan GetOverlap(TimeSpan rangeStart, TimeSpan rangeEnd, TimeSpan otherStart, TimeSpan otherEnd)
    {
        var overlapStart = rangeStart > otherStart ? rangeStart : otherStart;
        var overlapEnd = rangeEnd < otherEnd ? rangeEnd : otherEnd;
        var overlap = overlapEnd - overlapStart;
 
        return overlap > TimeSpan.Zero ? overlap : TimeSpan.Zero;
    }
 
    // Sums up only the parts of Lunch/Break that actually fall inside the given working window.
    // A break that lies completely before "start" or after "end" is ignored, as requested.
    private TimeSpan GetBreakDurationWithin(TimeSpan start, TimeSpan end)
    {
        var lunchOverlap = GetOverlap(start, end, TimeSpan.Parse(_settings.Lunch.Start), TimeSpan.Parse(_settings.Lunch.End));
        var breakOverlap = GetOverlap(start, end, TimeSpan.Parse(_settings.Break.Start), TimeSpan.Parse(_settings.Break.End));
 
        return lunchOverlap + breakOverlap;
    }
 
    // Overtime = actually worked time (end - start, minus breaks that fall within that window) - target work time.
    public TimeSpan CalculateOvertime(TimeSpan start, TimeSpan end)
    {
        var worked = end - start - GetBreakDurationWithin(start, end);
        var workTime = TimeSpan.Parse(_settings.WorkTime);
 
        return worked - workTime;
    }
 
    // Go-home time = start + target work time + breaks that fall inside the resulting window.
    public TimeSpan CalculateGoHomeTime(TimeSpan start) => CalculateEndTime(start, TimeSpan.Parse(_settings.WorkTime));
 
    // Latest allowed end of the day = start + legal/configured maximum work time + breaks within that window.
    public TimeSpan CalculateMaxEndTime(TimeSpan start) => CalculateEndTime(start, TimeSpan.Parse(_settings.MaximumWorkTime));
 
    // Shared fixed-point calculation: end = start + targetDuration + breaks that fall within [start, end].
    // Since adding a break can shift "end" further into (or newly into) a break, this is solved
    // iteratively: recompute "end" until adding the currently overlapping breaks no longer
    // changes it. Converges after at most a couple of iterations for 2 fixed breaks.
    private TimeSpan CalculateEndTime(TimeSpan start, TimeSpan targetDuration)
    {
        var end = start + targetDuration;
 
        TimeSpan previousEnd;
        do
        {
            previousEnd = end;
            end = start + targetDuration + GetBreakDurationWithin(start, end);
        } while (end != previousEnd);
 
        return end;
    }
}