using Cotizacion.Application.Common;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Cotizaciones.AgregarPartida;

public sealed class AgregarPartidaCommandHandler(
    ICotizacionRepository cotizacionRepository,
    IItemCatalogoRepository itemCatalogoRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AgregarPartidaCommand, Unit>
{
    public async Task<Unit> Handle(AgregarPartidaCommand request, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.GetByIdWithPartidasAsync(request.CotizacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cotizacion), request.CotizacionId);

        _ = await itemCatalogoRepository.GetByIdAsync(request.ItemCatalogoId, cancellationToken)
            ?? throw new NotFoundException(nameof(ItemCatalogo), request.ItemCatalogoId);

        cotizacion.AgregarPartida(request.ItemCatalogoId, request.PrecioUnitario, request.Cantidad);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
