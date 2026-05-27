using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.ActualizarActividad;

public sealed class ActualizarActividadCommandHandler(
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarActividadCommand, ActividadDto>
{
    public async Task<ActividadDto> Handle(ActualizarActividadCommand request, CancellationToken cancellationToken)
    {
        var actividad = await actividadRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Actividad), request.Id);

        actividad.Actualizar(request.Nombre, request.Orden);
        actividadRepository.Update(actividad);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ActividadDto(actividad.Id, actividad.Codigo, actividad.Nombre, actividad.Orden, actividad.Activo);
    }
}
