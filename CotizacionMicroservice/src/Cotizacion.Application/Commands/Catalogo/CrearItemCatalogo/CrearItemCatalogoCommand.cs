using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;

public record CrearItemCatalogoCommand(
    Guid ActividadId,
    string Codigo,
    string Descripcion,
    string Unidad,
    decimal PrecioBase) : IRequest<ItemCatalogoDto>;
