using Cotizacion.Domain.Common;
using Cotizacion.Domain.Enums;

namespace Cotizacion.Domain.Events;

public sealed record CotizacionEstadoCambiadoEvent(
    Guid CotizacionId,
    EstadoCotizacion EstadoAnterior,
    EstadoCotizacion EstadoNuevo) : DomainEvent;
