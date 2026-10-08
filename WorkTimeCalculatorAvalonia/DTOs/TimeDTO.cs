using System.Text.Json.Serialization;

namespace WorkTimeCalculatorAvalonia.DTOs;

public class TimeDTO
{
    [JsonPropertyName("WorkTime")]
    public string WorkTime { get; set; } = string.Empty;
    
    [JsonPropertyName("MaximumWorkTime")]
    public string MaximumWorkTime { get; set; } = string.Empty;
    
    [JsonPropertyName("Lunch")]
    public TimeRangeDTO Lunch { get; set; } = new();
    
    [JsonPropertyName("Break")]
    public TimeRangeDTO Break { get; set; } = new();
}