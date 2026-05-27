using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GetCotizacionById;

public sealed class GetCotizacionByIdQueryHandler(
    ICotizacionRepository cotizacionRepository,
    IClienteRepository clienteRepository) : IRequestHandler<GetCotizacionByIdQuery, CotizacionDetalleDto>
{
    public async Task<CotizacionDetalleDto> Handle(GetCotizacionByIdQuery request, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.GetByIdWithPartidasAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cotizacion), request.Id);

        var cliente = await clienteRepository.GetByIdAsync(cotizacion.ClienteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cliente), cotizacion.ClienteId);

        var clienteDto = new ClienteDto(cliente.Id, cliente.Nombre, cliente.Ruc, cliente.Email, cliente.Telefono, cliente.Direccion, cliente.Activo);

        var partidas = cotizacion.Partidas.Select(p => new CotizacionPartidaDto(
            p.Id,
            p.ItemCatalogoId,
            p.ItemCatalogo?.Codigo ?? string.Empty,
            p.ItemCatalogo?.Descripcion ?? string.Empty,
            p.ItemCatalogo?.Unidad ?? string.Empty,
            p.ItemCatalogo?.Actividad?.Nombre ?? string.Empty,
            p.PrecioUnitario,
            p.Cantidad,
            p.Subtotal)).ToList();

        return new CotizacionDetalleDto(cotizacion.Id, cotizacion.Numero, cotizacion.Proyecto, cotizacion.Ubicacion,
            clienteDto, cotizacion.Fecha, cotizacion.FechaValidez, cotizacion.Estado, cotizacion.Moneda,
            cotizacion.Notas, cotizacion.Total, partidas);
    }
}
