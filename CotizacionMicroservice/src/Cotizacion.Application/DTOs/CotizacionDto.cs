using Cotizacion.Domain.Enums;

namespace Cotizacion.Application.DTOs;

public record CotizacionResumenDto(
    Guid Id,
    string Numero,
    string Proyecto,
    string? Ubicacion,
    string Cliente,
    DateOnly Fecha,
    DateOnly? FechaValidez,
    EstadoCotizacion Estado,
    Moneda Moneda,
    decimal Total);

public record CotizacionDetalleDto(
    Guid Id,
    string Numero,
    string Proyecto,
    string? Ubicacion,
    ClienteDto Cliente,
    DateOnly Fecha,
    DateOnly? FechaValidez,
    EstadoCotizacion Estado,
    Moneda Moneda,
    string? Notas,
    decimal Total,
    IReadOnlyList<CotizacionPartidaDto> Partidas);

public record CotizacionPartidaDto(
    Guid Id,
    Guid ItemCatalogoId,
    string ItemCodigo,
    string ItemDescripcion,
    string ItemUnidad,
    string ActividadNombre,
    decimal PrecioUnitario,
    decimal? Cantidad,
    decimal Subtotal);
