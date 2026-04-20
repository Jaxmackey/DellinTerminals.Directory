using DellinTerminals.Domain.Entities;
using MediatR;

namespace DellinTerminals.Application.Terminals.Queries.FindOfficesByCity;

public record FindOfficesByCityQuery(
    string CityName,
    string? RegionName = null) : IRequest<IEnumerable<Office>>;