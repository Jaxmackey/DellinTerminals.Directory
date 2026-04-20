using System.Text.Json.Serialization;

namespace DellinTerminals.Domain.Entities;

public class JsonPhone
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [JsonPropertyName("additional")]
    public string? Additional { get; set; }
}