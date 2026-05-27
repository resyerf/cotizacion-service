using Cotizacion.Application.DTOs;
using Cotizacion.Application.Services;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GenerarPdf;

public sealed class GenerarPdfCotizacionQueryHandler(
    ICotizacionRepository cotizacionRepository,
    IClienteRepository clienteRepository,
    IPdfService pdfService) : IRequestHandler<GenerarPdfCotizacionQuery, PdfResult>
{
    public async Task<PdfResult> Handle(GenerarPdfCotizacionQuery request, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.GetByIdWithPartidasAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cotizacion), request.Id);

        var cliente = await clienteRepository.GetByIdAsync(cotizacion.ClienteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cliente), cotizacion.ClienteId);

        var clienteDto = new ClienteDto(
            cliente.Id, cliente.Nombre, cliente.Ruc,
            cliente.Email, cliente.Telefono, cliente.Direccion, cliente.Activo);

        var partidas = cotizacion.Partidas
            .OrderBy(p => p.ItemCatalogo?.Actividad?.Orden ?? int.MaxValue)
            .Select(p => new CotizacionPartidaDto(
                p.Id,
                p.ItemCatalogoId,
                p.ItemCatalogo?.Codigo ?? string.Empty,
                p.ItemCatalogo?.Descripcion ?? string.Empty,
                p.ItemCatalogo?.Unidad ?? string.Empty,
                p.ItemCatalogo?.Actividad?.Nombre ?? string.Empty,
                p.ItemCatalogo?.Actividad?.Orden ?? 0,
                p.PrecioUnitario,
                p.Cantidad,
                p.Subtotal)).ToList();

        var dto = new CotizacionDetalleDto(
            cotizacion.Id, cotizacion.Numero, cotizacion.Proyecto, cotizacion.Ubicacion,
            clienteDto, cotizacion.Fecha, cotizacion.FechaValidez, cotizacion.Estado, cotizacion.Moneda,
            cotizacion.Notas, cotizacion.Total, partidas);

        var bytes = pdfService.GenerarCotizacion(dto);
        var filename = $"COT-{cotizacion.Numero}-{SanitizeFilename(cotizacion.Proyecto)}.pdf";

        return new PdfResult(bytes, filename);
    }

    private static string SanitizeFilename(string name) =>
        string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c))
              .Replace(' ', '-');
}
