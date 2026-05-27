using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Catalogo.GetActividades;

public sealed class GetActividadesQueryHandler(IActividadRepository actividadRepository)
    : IRequestHandler<GetActividadesQuery, IEnumerable<ActividadDto>>
{
    public async Task<IEnumerable<ActividadDto>> Handle(GetActividadesQuery request, CancellationToken cancellationToken)
    {
        var actividades = await actividadRepository.GetAllAsync(cancellationToken);
        return actividades
            .OrderBy(a => a.Orden)
            .Select(a => new ActividadDto(a.Id, a.Codigo, a.Nombre, a.Orden, a.Activo));
    }
}
