using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GetCotizacionById;

public record GetCotizacionByIdQuery(Guid Id) : IRequest<CotizacionDetalleDto>;
