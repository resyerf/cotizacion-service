using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.ActualizarItemCatalogo;

public sealed class ActualizarItemCatalogoCommandHandler(
    IItemCatalogoRepository itemRepository,
    IActividadRepository actividadRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarItemCatalogoCommand, ItemCatalogoDto>
{
    public async Task<ItemCatalogoDto> Handle(ActualizarItemCatalogoCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ItemCatalogo), request.Id);

        var actividad = await actividadRepository.GetByIdAsync(request.ActividadId, cancellationToken)
            ?? throw new NotFoundException(nameof(Actividad), request.ActividadId);

        item.Actualizar(request.ActividadId, request.Descripcion, request.Unidad, request.PrecioBase, request.Moneda);
        itemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ItemCatalogoDto(item.Id, item.ActividadId, actividad.Nombre,
            item.Codigo, item.Descripcion, item.Unidad, item.PrecioBase, item.Moneda, item.Activo);
    }
}
