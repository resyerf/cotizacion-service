using Cotizacion.Application.Common;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Cotizaciones.CambiarEstado;

public sealed class CambiarEstadoCotizacionCommandHandler(
    ICotizacionRepository cotizacionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CambiarEstadoCotizacionCommand, Unit>
{
    public async Task<Unit> Handle(CambiarEstadoCotizacionCommand request, CancellationToken cancellationToken)
    {
        var cotizacion = await cotizacionRepository.GetByIdAsync(request.CotizacionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cotizacion), request.CotizacionId);

        cotizacion.CambiarEstado(request.NuevoEstado);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
