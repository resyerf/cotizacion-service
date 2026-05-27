using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Enums;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.ActualizarItemCatalogo;

public record ActualizarItemCatalogoCommand(
    Guid Id,
    Guid ActividadId,
    string Descripcion,
    string Unidad,
    decimal PrecioBase,
    Moneda Moneda) : IRequest<ItemCatalogoDto>;
