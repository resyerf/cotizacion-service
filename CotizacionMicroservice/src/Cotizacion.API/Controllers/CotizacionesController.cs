using Cotizacion.Application.Commands.Cotizaciones.AgregarPartida;
using Cotizacion.Application.Commands.Cotizaciones.CambiarEstado;
using Cotizacion.Application.Commands.Cotizaciones.CrearCotizacion;
using Cotizacion.Application.DTOs;
using Cotizacion.Application.Queries.Cotizaciones.GenerarExcel;
using Cotizacion.Application.Queries.Cotizaciones.GenerarPdf;
using Cotizacion.Application.Queries.Cotizaciones.GetCotizacionById;
using Cotizacion.Application.Queries.Cotizaciones.GetCotizaciones;
using Cotizacion.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

/// <summary>Gestión de cotizaciones comerciales.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class CotizacionesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todas las cotizaciones, con filtros opcionales.</summary>
    /// <param name="clienteId">Filtra por cliente (opcional).</param>
    /// <param name="estado">Filtra por estado de cotización (opcional).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Lista de cotizaciones.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CotizacionResumenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? clienteId,
        [FromQuery] EstadoCotizacion? estado,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotizacionesQuery(clienteId, estado), cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene el detalle completo de una cotización por su ID.</summary>
    /// <param name="id">ID de la cotización.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Detalle de la cotización con partidas.</response>
    /// <response code="404">Cotización no encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CotizacionDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotizacionByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea una nueva cotización.</summary>
    /// <response code="201">Cotización creada exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    /// <response code="404">Cliente no encontrado.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CotizacionResumenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CrearCotizacionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Agrega una partida (línea de ítem) a una cotización existente.</summary>
    /// <param name="id">ID de la cotización.</param>
    /// <param name="request">Datos de la partida a agregar.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Partida agregada exitosamente.</response>
    /// <response code="400">Datos inválidos o cotización no editable.</response>
    /// <response code="404">Cotización o ítem de catálogo no encontrado.</response>
    [HttpPost("{id:guid}/partidas")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AgregarPartida(
        Guid id,
        [FromBody] AgregarPartidaRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new AgregarPartidaCommand(id, request.ItemCatalogoId, request.PrecioUnitario, request.Cantidad), cancellationToken);
        return NoContent();
    }

    /// <summary>Cambia el estado de una cotización.</summary>
    /// <param name="id">ID de la cotización.</param>
    /// <param name="request">Nuevo estado a aplicar.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="204">Estado actualizado exitosamente.</response>
    /// <response code="400">Transición de estado inválida.</response>
    /// <response code="404">Cotización no encontrada.</response>
    /// <response code="422">Violación de regla de negocio.</response>
    [HttpPatch("{id:guid}/estado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CambiarEstado(
        Guid id,
        [FromBody] CambiarEstadoRequest request,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CambiarEstadoCotizacionCommand(id, request.NuevoEstado), cancellationToken);
        return NoContent();
    }

    /// <summary>Genera y descarga la cotización en formato PDF.</summary>
    /// <param name="id">ID de la cotización.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Archivo PDF de la cotización.</response>
    /// <response code="404">Cotización no encontrada.</response>
    [HttpGet("{id:guid}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportarPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerarPdfCotizacionQuery(id), cancellationToken);
        return File(result.Bytes, "application/pdf", result.Filename);
    }

    /// <summary>Genera y descarga la cotización en formato Excel.</summary>
    /// <param name="id">ID de la cotización.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Archivo Excel de la cotización.</response>
    /// <response code="404">Cotización no encontrada.</response>
    [HttpGet("{id:guid}/excel")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportarExcel(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerarExcelCotizacionQuery(id), cancellationToken);
        return File(result.Bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Filename);
    }
}

/// <summary>Datos para agregar una partida a una cotización.</summary>
/// <param name="ItemCatalogoId">ID del ítem del catálogo.</param>
/// <param name="PrecioUnitario">Precio unitario acordado para esta partida.</param>
/// <param name="Cantidad">Cantidad de unidades (opcional).</param>
public record AgregarPartidaRequest(Guid ItemCatalogoId, decimal PrecioUnitario, decimal? Cantidad);

/// <summary>Datos para cambiar el estado de una cotización.</summary>
/// <param name="NuevoEstado">Estado destino de la cotización.</param>
public record CambiarEstadoRequest(EstadoCotizacion NuevoEstado);
