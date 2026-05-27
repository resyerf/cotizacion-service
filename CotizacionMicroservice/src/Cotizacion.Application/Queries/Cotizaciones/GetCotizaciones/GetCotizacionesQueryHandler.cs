using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GetCotizaciones;

public sealed class GetCotizacionesQueryHandler(
    ICotizacionRepository cotizacionRepository,
    IClienteRepository clienteRepository) : IRequestHandler<GetCotizacionesQuery, IEnumerable<CotizacionResumenDto>>
{
    public async Task<IEnumerable<CotizacionResumenDto>> Handle(GetCotizacionesQuery request, CancellationToken cancellationToken)
    {
        var cotizaciones = request.ClienteId.HasValue
            ? await cotizacionRepository.GetByClienteIdAsync(request.ClienteId.Value, cancellationToken)
            : await cotizacionRepository.GetAllAsync(cancellationToken);

        if (request.Estado.HasValue)
            cotizaciones = cotizaciones.Where(c => c.Estado == request.Estado.Value);

        var clientes = (await clienteRepository.GetAllAsync(cancellationToken))
            .ToDictionary(c => c.Id, c => c.Nombre);

        return cotizaciones.Select(c => new CotizacionResumenDto(
            c.Id, c.Numero, c.Proyecto, c.Ubicacion,
            clientes.GetValueOrDefault(c.ClienteId, "Desconocido"),
            c.Fecha, c.FechaValidez, c.Estado, c.Moneda, c.Total));
    }
}
