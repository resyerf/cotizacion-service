using Cotizacion.Domain.Common;

namespace Cotizacion.Domain.Events;

public sealed record CotizacionCreadaEvent(Guid CotizacionId, string Numero, Guid ClienteId) : DomainEvent;
