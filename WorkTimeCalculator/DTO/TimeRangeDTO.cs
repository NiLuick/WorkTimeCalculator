using System.Text.Json.Serialization;

namespace WorkTimeCalculator.DTO;

public class TimeRangeDTO
{
    [JsonPropertyName("Start")]
    public string Start { get; set; } = string.Empty;

    [JsonPropertyName("End")]
    public string End { get; set; } = string.Empty;
}