using Cotizacion.Application.Commands.Catalogo.ActualizarActividad;
using Cotizacion.Application.Commands.Catalogo.ActualizarItemCatalogo;
using Cotizacion.Application.Commands.Catalogo.CrearActividad;
using Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;
using Cotizacion.Application.Commands.Catalogo.DesactivarActividad;
using Cotizacion.Application.Commands.Catalogo.DesactivarItemCatalogo;
using Cotizacion.Application.DTOs;
using Cotizacion.Application.Queries.Catalogo.GetActividades;
using Cotizacion.Application.Queries.Catalogo.GetItemsCatalogo;
using Cotizacion.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

/// <summary>Gestión del catálogo de actividades e ítems de servicio.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class CatalogoController(IMediator mediator) : ControllerBase
{
    // ── Actividades ──────────────────────────────────────────────

    /// <summary>Lista todas las actividades del catálogo.</summary>
    [HttpGet("actividades")]
    [ProducesResponseType(typeof(IEnumerable<ActividadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActividades(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetActividadesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva actividad en el catálogo.</summary>
    [HttpPost("actividades")]
    [ProducesResponseType(typeof(ActividadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateActividad([FromBody] CrearActividadCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/actividades/{result.Id}", result);
    }

    /// <summary>Actualiza una actividad del catálogo.</summary>
    [HttpPut("actividades/{id:guid}")]
    [ProducesResponseType(typeof(ActividadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActividad(Guid id, [FromBody] ActualizarActividadBody body, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ActualizarActividadCommand(id, body.Nombre, body.Orden),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Desactiva una actividad (borrado lógico).</summary>
    [HttpDelete("actividades/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateActividad(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DesactivarActividadCommand(id), cancellationToken);
        return NoContent();
    }

    // ── Items ────────────────────────────────────────────────────

    /// <summary>Lista los ítems del catálogo, opcionalmente filtrados por actividad.</summary>
    [HttpGet("items")]
    [ProducesResponseType(typeof(IEnumerable<ItemCatalogoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItems([FromQuery] Guid? actividadId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetItemsCatalogoQuery(actividadId), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea un nuevo ítem en el catálogo.</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(ItemCatalogoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateItem([FromBody] CrearItemCatalogoCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Created($"api/catalogo/items/{result.Id}", result);
    }

    /// <summary>Actualiza un ítem del catálogo.</summary>
    [HttpPut("items/{id:guid}")]
    [ProducesResponseType(typeof(ItemCatalogoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ActualizarItemCatalogoBody body, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ActualizarItemCatalogoCommand(id, body.ActividadId, body.Descripcion, body.Unidad, body.PrecioBase, body.Moneda),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Desactiva un ítem del catálogo (borrado lógico).</summary>
    [HttpDelete("items/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateItem(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DesactivarItemCatalogoCommand(id), cancellationToken);
        return NoContent();
    }
}

public record ActualizarActividadBody(string Nombre, int Orden);

public record ActualizarItemCatalogoBody(
    Guid ActividadId,
    string Descripcion,
    string Unidad,
    decimal PrecioBase,
    Moneda Moneda);
