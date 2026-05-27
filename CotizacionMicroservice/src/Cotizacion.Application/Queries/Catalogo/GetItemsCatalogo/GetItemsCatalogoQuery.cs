using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Queries.Catalogo.GetItemsCatalogo;

public record GetItemsCatalogoQuery(Guid? ActividadId = null) : IRequest<IEnumerable<ItemCatalogoDto>>;
