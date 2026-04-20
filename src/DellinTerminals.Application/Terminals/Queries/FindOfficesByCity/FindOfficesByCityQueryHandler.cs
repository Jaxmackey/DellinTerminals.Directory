using DellinTerminals.Domain.Entities;
using MediatR;
using DellinTerminals.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DellinTerminals.Application.Terminals.Queries.FindOfficesByCity;

public class FindOfficesByCityQueryHandler : IRequestHandler<FindOfficesByCityQuery, IEnumerable<Office>>
{
    private readonly IOfficeRepository _repository;
    private readonly ILogger<FindOfficesByCityQueryHandler> _logger;

    public FindOfficesByCityQueryHandler(
        IOfficeRepository repository,
        ILogger<FindOfficesByCityQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Office>> Handle(
        FindOfficesByCityQuery request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching offices for city: {CityName}, region: {RegionName}", 
            request.CityName, request.RegionName);

        var offices = await _repository.GetOfficesByCityAsync(
            request.CityName, 
            request.RegionName, 
            cancellationToken);

        var result = offices.Select(o => new Office
        {
            Id = o.Id,
            Code = o.Code,
            CityCode = o.CityCode,
            Uuid = o.Uuid,
            Type = o.Type,
            CountryCode = o.CountryCode,
            Coordinates = new Coordinates
            {
                Latitude = o.Coordinates.Latitude,
                Longitude = o.Coordinates.Longitude
            },
    
            AddressRegion = o.AddressRegion,
            AddressCity = o.AddressCity,
            AddressStreet = o.AddressStreet,
            AddressHouseNumber = o.AddressHouseNumber,
            AddressApartment = o.AddressApartment,
            WorkTime = o.WorkTime,
            Phones = o.Phones.Select(p => new Phone
            {
                PhoneNumber = p.PhoneNumber,
                Additional = p.Additional
            })
        });

        _logger.LogInformation("Found {Count} offices", result.Count());
        return result;
    }
}