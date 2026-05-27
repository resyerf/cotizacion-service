using Cotizacion.Application.Commands.Cotizaciones.AgregarPartida;
using Cotizacion.Application.Commands.Cotizaciones.CambiarEstado;
using Cotizacion.Application.Commands.Cotizaciones.CrearCotizacion;
using Cotizacion.Application.Queries.Cotizaciones.GenerarExcel;
using Cotizacion.Application.Queries.Cotizaciones.GenerarPdf;
using Cotizacion.Application.Queries.Cotizaciones.GetCotizacionById;
using Cotizacion.Application.Queries.Cotizaciones.GetCotizaciones;
using Cotizacion.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CotizacionesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? clienteId, [FromQuery] EstadoCotizacion? estado, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotizacionesQuery(clienteId, estado), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCotizacionByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCotizacionCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/partidas")]
    public async Task<IActionResult> AgregarPartida(Guid id, [FromBody] AgregarPartidaRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new AgregarPartidaCommand(id, request.ItemCatalogoId, request.PrecioUnitario, request.Cantidad), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(new CambiarEstadoCotizacionCommand(id, request.NuevoEstado), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> ExportarPdf(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerarPdfCotizacionQuery(id), cancellationToken);
        return File(result.Bytes, "application/pdf", result.Filename);
    }

    [HttpGet("{id:guid}/excel")]
    public async Task<IActionResult> ExportarExcel(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GenerarExcelCotizacionQuery(id), cancellationToken);
        return File(result.Bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            result.Filename);
    }
}

public record AgregarPartidaRequest(Guid ItemCatalogoId, decimal PrecioUnitario, decimal? Cantidad);
public record CambiarEstadoRequest(EstadoCotizacion NuevoEstado);
