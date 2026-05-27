using Cotizacion.Domain.Enums;

namespace Cotizacion.Application.DTOs;

public record ItemCatalogoDto(
    Guid Id,
    Guid ActividadId,
    string ActividadNombre,
    string Codigo,
    string Descripcion,
    string Unidad,
    decimal PrecioBase,
    Moneda Moneda,
    bool Activo);
