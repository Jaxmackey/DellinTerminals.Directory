using DellinTerminals.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using DellinTerminals.Application.Terminals.Queries.FindOfficesByCity;
using DellinTerminals.Application.Terminals.Queries.GetCityIdByOffice;
using DellinTerminals.Api.Mappers;

namespace DellinTerminals.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TerminalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TerminalsController> _logger;

    public TerminalsController(IMediator mediator, ILogger<TerminalsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Поиск терминалов города по названию города и области
    /// </summary>
    [HttpGet("offices")]
    [ProducesResponseType(typeof(IEnumerable<OfficeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchOffices(
        [FromQuery] string city, 
        [FromQuery] string? region = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { error = "Параметр 'city' обязателен" });

        _logger.LogInformation("API: Поиск офисов по городу={City}, регион={Region}", city, region);
        
        var query = new FindOfficesByCityQuery(city.Trim(), region?.Trim());
        var domainResult = await _mediator.Send(query, ct);
        
        var apiResult = domainResult.Select(o => o.ToResponse());
        
        return Ok(apiResult);
    }

    /// <summary>
    /// Поиск идентификатора города по названию города и области
    /// </summary>
    [HttpGet("city-id")]
    [ProducesResponseType(typeof(CityIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCityId(
        [FromQuery] string city, 
        [FromQuery] string? region = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { error = "Параметр 'city' обязателен" });

        _logger.LogInformation("API: Поиск CityId по городу={City}, регион={Region}", city, region);
        
        var query = new GetCityIdByOfficeQuery(city.Trim(), region?.Trim());
        var cityId = await _mediator.Send(query, ct);
        
        if (!cityId.HasValue)
            return NotFound(new { error = "Город не найден в справочнике" });
        return Ok(new CityIdResponse(cityId.Value));
    }
}