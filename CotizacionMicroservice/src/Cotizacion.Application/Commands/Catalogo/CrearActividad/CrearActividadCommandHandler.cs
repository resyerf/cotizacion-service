using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.CrearActividad;

public sealed class CrearActividadCommandHandler(
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearActividadCommand, ActividadDto>
{
    public async Task<ActividadDto> Handle(CrearActividadCommand request, CancellationToken cancellationToken)
    {
        var todas = await actividadRepository.GetAllAsync(cancellationToken);
        var orden = todas.Count() + 1;

        // Auto-generate a unique short code
        string codigo;
        do { codigo = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(); }
        while (await actividadRepository.ExisteCodigoAsync(codigo, cancellationToken: cancellationToken));

        var actividad = Actividad.Crear(codigo, request.Nombre, orden);
        await actividadRepository.AddAsync(actividad, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ActividadDto(actividad.Id, actividad.Codigo, actividad.Nombre, actividad.Orden, actividad.Activo);
    }
}
