using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Catalogo.GetItemsCatalogo;

public sealed class GetItemsCatalogoQueryHandler(
    IItemCatalogoRepository itemRepository,
    IActividadRepository actividadRepository) : IRequestHandler<GetItemsCatalogoQuery, IEnumerable<ItemCatalogoDto>>
{
    public async Task<IEnumerable<ItemCatalogoDto>> Handle(GetItemsCatalogoQuery request, CancellationToken cancellationToken)
    {
        var items = request.ActividadId.HasValue
            ? await itemRepository.GetByActividadIdAsync(request.ActividadId.Value, cancellationToken)
            : await itemRepository.GetAllAsync(cancellationToken);

        var actividades = (await actividadRepository.GetAllAsync(cancellationToken))
            .ToDictionary(a => a.Id, a => a.Nombre);

        return items
            .Where(i => i.Activo)
            .Select(i => new ItemCatalogoDto(i.Id, i.ActividadId,
                actividades.GetValueOrDefault(i.ActividadId, string.Empty),
                i.Codigo, i.Descripcion, i.Unidad, i.PrecioBase, i.Activo));
    }
}
