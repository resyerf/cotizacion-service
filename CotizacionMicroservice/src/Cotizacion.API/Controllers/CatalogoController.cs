using Cotizacion.Application.Commands.Catalogo.CrearActividad;
using Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;
using Cotizacion.Application.Queries.Catalogo.GetActividades;
using Cotizacion.Application.Queries.Catalogo.GetItemsCatalogo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CatalogoController(IMediator mediator) : ControllerBase
{
    [HttpGet("actividades")]
    public async Task<IActionResult> GetActividades(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActividadesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("actividades")]
    public async Task<IActionResult> CreateActividad([FromBody] CrearActividadCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/actividades/{result.Id}", result);
    }

    [HttpGet("items")]
    public async Task<IActionResult> GetItems([FromQuery] Guid? actividadId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetItemsCatalogoQuery(actividadId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem([FromBody] CrearItemCatalogoCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/items/{result.Id}", result);
    }
}
