using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.CrearActividad;

public sealed class CrearActividadCommandHandler(
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearActividadCommand, ActividadDto>
{
    public async Task<ActividadDto> Handle(CrearActividadCommand request, CancellationToken cancellationToken)
    {
        if (await actividadRepository.ExisteCodigoAsync(request.Codigo, cancellationToken: cancellationToken))
            throw new DomainException($"Ya existe una actividad con código '{request.Codigo}'.");

        var actividad = Actividad.Crear(request.Codigo, request.Nombre, request.Orden);
        await actividadRepository.AddAsync(actividad, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ActividadDto(actividad.Id, actividad.Codigo, actividad.Nombre, actividad.Orden, actividad.Activo);
    }
}
