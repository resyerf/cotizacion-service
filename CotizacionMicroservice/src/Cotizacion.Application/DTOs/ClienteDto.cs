namespace Cotizacion.Application.DTOs;

public record ClienteDto(
    Guid Id,
    string Nombre,
    string? Ruc,
    string? Email,
    string? Telefono,
    string? Direccion,
    bool Activo);
