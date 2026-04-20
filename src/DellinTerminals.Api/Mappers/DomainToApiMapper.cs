using DellinTerminals.Api.Contracts;
using DellinTerminals.Domain.Entities;

namespace DellinTerminals.Api.Mappers;

public static class DomainToApiMapper
{
    public static OfficeResponse ToResponse(this Office domain)
    {
        return new OfficeResponse
        {
            Id = domain.Id,
            Code = domain.Code,
            CityCode = domain.CityCode,
            Uuid = domain.Uuid,
            Type = domain.Type?.ToString(),
            CountryCode = domain.CountryCode,
            Coordinates = new CoordinatesResponse
            {
                Latitude = domain.Coordinates.Latitude,
                Longitude = domain.Coordinates.Longitude
            },
            
            AddressRegion = domain.AddressRegion,
            AddressCity = domain.AddressCity,
            AddressStreet = domain.AddressStreet,
            AddressHouseNumber = domain.AddressHouseNumber,
            AddressApartment = domain.AddressApartment,
            WorkTime = domain.WorkTime,
            Phones = domain.Phones.Select(p => p.ToResponse())
        };
    }

    public static PhoneResponse ToResponse(this Phone domain)
    {
        return new PhoneResponse
        {
            PhoneNumber = domain.PhoneNumber,
            Additional = domain.Additional
        };
    }

    public static CityIdResponse ToResponse(int cityId)
    {
        return new CityIdResponse(cityId);
    }
}