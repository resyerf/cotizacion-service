using Cotizacion.Domain.Enums;
using MediatR;

namespace Cotizacion.Application.Commands.Cotizaciones.CambiarEstado;

public record CambiarEstadoCotizacionCommand(Guid CotizacionId, EstadoCotizacion NuevoEstado) : IRequest<Unit>;
