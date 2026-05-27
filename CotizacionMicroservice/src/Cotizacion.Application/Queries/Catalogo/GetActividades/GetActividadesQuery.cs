using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Queries.Catalogo.GetActividades;

public record GetActividadesQuery : IRequest<IEnumerable<ActividadDto>>;
