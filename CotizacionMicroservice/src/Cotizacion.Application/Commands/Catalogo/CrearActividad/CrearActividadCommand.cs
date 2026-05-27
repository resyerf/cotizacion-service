using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.CrearActividad;

public record CrearActividadCommand(string Codigo, string Nombre, int Orden) : IRequest<ActividadDto>;
