using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Enums;
using MediatR;

namespace Cotizacion.Application.Commands.Cotizaciones.CrearCotizacion;

public record CrearCotizacionCommand(
    Guid ClienteId,
    string Proyecto,
    string? Ubicacion,
    DateOnly Fecha,
    DateOnly? FechaValidez,
    Moneda Moneda,
    string? Notas) : IRequest<CotizacionResumenDto>;
