using Cotizacion.Application.Commands.Catalogo.CrearActividad;
using Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;
using Cotizacion.Application.DTOs;
using Cotizacion.Application.Queries.Catalogo.GetActividades;
using Cotizacion.Application.Queries.Catalogo.GetItemsCatalogo;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

/// <summary>Gestión del catálogo de actividades e ítems de servicio.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class CatalogoController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todas las actividades del catálogo.</summary>
    /// <response code="200">Lista de actividades.</response>
    [HttpGet("actividades")]
    [ProducesResponseType(typeof(IEnumerable<ActividadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActividades(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActividadesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva actividad en el catálogo.</summary>
    /// <response code="201">Actividad creada exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost("actividades")]
    [ProducesResponseType(typeof(ActividadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateActividad([FromBody] CrearActividadCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/actividades/{result.Id}", result);
    }

    /// <summary>Lista los ítems del catálogo, opcionalmente filtrados por actividad.</summary>
    /// <param name="actividadId">ID de la actividad para filtrar (opcional).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Lista de ítems del catálogo.</response>
    [HttpGet("items")]
    [ProducesResponseType(typeof(IEnumerable<ItemCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItems([FromQuery] Guid? actividadId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetItemsCatalogoQuery(actividadId), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea un nuevo ítem en el catálogo.</summary>
    /// <response code="201">Ítem creado exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(ItemCatalogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateItem([FromBody] CrearItemCatalogoCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/items/{result.Id}", result);
    }
}
