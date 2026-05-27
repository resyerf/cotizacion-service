namespace Cotizacion.Application.DTOs;

public record ActividadDto(
    Guid Id,
    string Codigo,
    string Nombre,
    int Orden,
    bool Activo);
