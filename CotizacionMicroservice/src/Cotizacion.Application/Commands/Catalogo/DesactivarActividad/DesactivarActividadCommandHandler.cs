using Cotizacion.Application.Common;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.DesactivarActividad;

public sealed class DesactivarActividadCommandHandler(
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DesactivarActividadCommand>
{
    public async Task Handle(DesactivarActividadCommand request, CancellationToken cancellationToken)
    {
        var actividad = await actividadRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Actividad), request.Id);

        actividad.Desactivar();
        actividadRepository.Update(actividad);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
