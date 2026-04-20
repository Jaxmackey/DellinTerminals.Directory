using System.Text.Json.Serialization;
using DellinTerminals.Domain.Enums;

namespace DellinTerminals.Domain.Entities;

public class JsonTerminal
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("cityCode")]
    public int CityCode { get; set; }
    
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OfficeType? Type { get; set; }
    
    [JsonPropertyName("countryCode")]
    public string CountryCode { get; set; } = string.Empty;
    
    [JsonPropertyName("coordinates")]
    public JsonCoordinates? Coordinates { get; set; }
    
    [JsonPropertyName("addressRegion")]
    public string? AddressRegion { get; set; }
    
    [JsonPropertyName("addressCity")]
    public string? AddressCity { get; set; }
    
    [JsonPropertyName("addressStreet")]
    public string? AddressStreet { get; set; }
    
    [JsonPropertyName("addressHouseNumber")]
    public string? AddressHouseNumber { get; set; }
    
    [JsonPropertyName("addressApartment")]
    public int? AddressApartment { get; set; }
    
    [JsonPropertyName("workTime")]
    public string WorkTime { get; set; } = string.Empty;
    
    [JsonPropertyName("phones")]
    public IEnumerable<JsonPhone>? Phones { get; set; }
}