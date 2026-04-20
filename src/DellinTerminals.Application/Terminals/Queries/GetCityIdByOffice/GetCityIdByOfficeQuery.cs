using MediatR;

namespace DellinTerminals.Application.Terminals.Queries.GetCityIdByOffice;

public record GetCityIdByOfficeQuery(
    string CityName,
    string? RegionName = null) : IRequest<int?>;