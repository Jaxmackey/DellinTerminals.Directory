using System.Text.Json.Serialization;

namespace DellinTerminals.Domain.Entities;

public class JsonCoordinates
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}