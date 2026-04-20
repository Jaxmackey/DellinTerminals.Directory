using DellinTerminals.Domain.Enums;

namespace DellinTerminals.Domain.Entities;

public class Office
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int CityCode { get; set; }
    public string? Uuid { get; set; }
    public OfficeType? Type { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public Coordinates Coordinates { get; set; } = new();
    public string? AddressRegion { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressStreet { get; set; }
    public string? AddressHouseNumber { get; set; }
    public int? AddressApartment { get; set; }
    public string WorkTime { get; set; } = string.Empty;
    public IEnumerable<Phone> Phones { get; set; } = new List<Phone>();
    public string NormalizedCityName { get; set; } = string.Empty;
}