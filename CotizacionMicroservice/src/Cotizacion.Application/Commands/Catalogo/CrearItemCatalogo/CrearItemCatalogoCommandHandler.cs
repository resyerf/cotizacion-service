using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;

public sealed class CrearItemCatalogoCommandHandler(
    IItemCatalogoRepository itemRepository,
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearItemCatalogoCommand, ItemCatalogoDto>
{
    public async Task<ItemCatalogoDto> Handle(CrearItemCatalogoCommand request, CancellationToken cancellationToken)
    {
        var actividad = await actividadRepository.GetByIdAsync(request.ActividadId, cancellationToken)
            ?? throw new NotFoundException(nameof(Actividad), request.ActividadId);

        if (await itemRepository.ExisteCodigoAsync(request.Codigo, cancellationToken: cancellationToken))
            throw new DomainException($"Ya existe un item con código '{request.Codigo}'.");

        var item = ItemCatalogo.Crear(request.ActividadId, request.Codigo, request.Descripcion, request.Unidad, request.PrecioBase);
        await itemRepository.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ItemCatalogoDto(item.Id, item.ActividadId, actividad.Nombre, item.Codigo, item.Descripcion, item.Unidad, item.PrecioBase, item.Activo);
    }
}
