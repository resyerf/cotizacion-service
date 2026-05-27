using MediatR;

namespace Cotizacion.Application.Commands.Cotizaciones.AgregarPartida;

public record AgregarPartidaCommand(
    Guid CotizacionId,
    Guid ItemCatalogoId,
    decimal PrecioUnitario,
    decimal? Cantidad) : IRequest<Unit>;
