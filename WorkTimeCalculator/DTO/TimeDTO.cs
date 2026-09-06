using System.Text.Json.Serialization;

namespace WorkTimeCalculator.DTO;

public class TimeDTO
{
    [JsonPropertyName("WorkTime")]
    public string WorkTime { get; set; } = string.Empty;
    
    [JsonPropertyName("Lunch")]
    public TimeRangeDTO Lunch { get; set; } = new();
    
    [JsonPropertyName("Break")]
    public TimeRangeDTO Break { get; set; } = new();
}