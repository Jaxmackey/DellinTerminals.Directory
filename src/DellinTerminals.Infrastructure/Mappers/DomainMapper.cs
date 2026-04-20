using DellinTerminals.Domain.Entities;
using DellinTerminals.Domain.Enums;
using DellinTerminals.Infrastructure.Data.Entities;

namespace DellinTerminals.Infrastructure.Mappers;

public static class DomainMapper
{
    public static Office ToDomain(this OfficeEntity entity)
    {
        return new Office
        {
            Id = entity.Id,
            Code = entity.Code,
            CityCode = entity.CityCode,
            Uuid = entity.Uuid,
            Type = string.IsNullOrEmpty(entity.Type) ? null : Enum.Parse<OfficeType>(entity.Type),
            CountryCode = entity.CountryCode,
            Coordinates = entity.Coordinates.ToDomain(),
            AddressRegion = entity.AddressRegion,
            AddressCity = entity.AddressCity,
            AddressStreet = entity.AddressStreet,
            AddressHouseNumber = entity.AddressHouseNumber,
            AddressApartment = entity.AddressApartment,
            WorkTime = entity.WorkTime,
            NormalizedCityName = entity.NormalizedCityName,
            Phones = entity.Phones.Select(p => p.ToDomain()).ToList()
        };
    }

    public static Coordinates ToDomain(this CoordinatesValueObject vo)
    {
        return new Coordinates(vo.Latitude, vo.Longitude);
    }

    public static Phone ToDomain(this PhoneEntity entity)
    {
        return new Phone
        {
            PhoneNumber = entity.PhoneNumber,
            Additional = entity.Additional
        };
    }
    
    public static OfficeEntity ToEntity(this Office domain)
    {
        var entity = new OfficeEntity
        {
            Id = domain.Id,
            Code = domain.Code,
            CityCode = domain.CityCode,
            Uuid = domain.Uuid,
            Type = domain.Type?.ToString(),
            CountryCode = domain.CountryCode,
            Coordinates = domain.Coordinates.ToEntity(),
            AddressRegion = domain.AddressRegion,
            AddressCity = domain.AddressCity,
            AddressStreet = domain.AddressStreet,
            AddressHouseNumber = domain.AddressHouseNumber,
            AddressApartment = domain.AddressApartment,
            WorkTime = domain.WorkTime,
            NormalizedCityName = domain.NormalizedCityName,
            Phones = new List<PhoneEntity>()
        };
        
        foreach (var phone in domain.Phones)
        {
            entity.Phones.Add(new PhoneEntity
            {
                OfficeId = entity.Id,
                PhoneNumber = phone.PhoneNumber,
                Additional = phone.Additional
            });
        }

        return entity;
    }

    public static CoordinatesValueObject ToEntity(this Coordinates domain)
    {
        return new CoordinatesValueObject
        {
            Latitude = domain.Latitude,
            Longitude = domain.Longitude
        };
    }
}