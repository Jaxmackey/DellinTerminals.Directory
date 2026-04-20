namespace DellinTerminals.Api.Contracts;

public class OfficeResponse
{
    public int Id { get; init; }
    public string? Code { get; init; }
    public int CityCode { get; init; }
    public string? Uuid { get; init; }
    public string? Type { get; init; }
    
    public string CountryCode { get; init; } = string.Empty;
    public CoordinatesResponse Coordinates { get; init; } = new();
    
    public string? AddressRegion { get; init; }
    public string? AddressCity { get; init; }
    public string? AddressStreet { get; init; }
    public string? AddressHouseNumber { get; init; }
    public int? AddressApartment { get; init; }
    public string WorkTime { get; init; } = string.Empty;
    
    public IEnumerable<PhoneResponse> Phones { get; init; } = Enumerable.Empty<PhoneResponse>();
}