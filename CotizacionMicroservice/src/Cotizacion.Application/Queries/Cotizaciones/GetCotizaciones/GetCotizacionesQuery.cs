using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Enums;
using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GetCotizaciones;

public record GetCotizacionesQuery(
    Guid? ClienteId = null,
    EstadoCotizacion? Estado = null) : IRequest<IEnumerable<CotizacionResumenDto>>;
