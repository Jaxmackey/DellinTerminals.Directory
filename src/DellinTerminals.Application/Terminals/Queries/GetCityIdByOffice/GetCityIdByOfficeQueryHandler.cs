using DellinTerminals.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DellinTerminals.Application.Terminals.Queries.GetCityIdByOffice;

public class GetCityIdByOfficeQueryHandler : IRequestHandler<GetCityIdByOfficeQuery, int?>
{
    private readonly IOfficeRepository _repository;
    private readonly ILogger<GetCityIdByOfficeQueryHandler> _logger;

    public GetCityIdByOfficeQueryHandler(
        IOfficeRepository repository,
        ILogger<GetCityIdByOfficeQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<int?> Handle(
        GetCityIdByOfficeQuery request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting city ID for: {CityName}, region: {RegionName}", 
            request.CityName, request.RegionName);

        var cityId = await _repository.GetCityIdByOfficeAsync(
            request.CityName, 
            request.RegionName, 
            cancellationToken);

        _logger.LogInformation("City ID result: {CityId}", cityId);
        return cityId;
    }
}