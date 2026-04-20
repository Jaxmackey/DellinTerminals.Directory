namespace DellinTerminals.Infrastructure.Data.Entities;

public class OfficeEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int CityCode { get; set; }
    public string? Uuid { get; set; }
    public string? Type { get; set; } // string для enum
    public string CountryCode { get; set; } = string.Empty;
    
    public CoordinatesValueObject Coordinates { get; set; } = new();
    
    public string? AddressRegion { get; set; }
    public string? AddressCity { get; set; }
    public string? AddressStreet { get; set; }
    public string? AddressHouseNumber { get; set; }
    public int? AddressApartment { get; set; }
    public string WorkTime { get; set; } = string.Empty;
    
    public string NormalizedCityName { get; set; } = string.Empty;
    
    // Навигация для EF
    public ICollection<PhoneEntity> Phones { get; set; } = new List<PhoneEntity>();
}