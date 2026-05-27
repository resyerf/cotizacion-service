using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.ActualizarActividad;

public record ActualizarActividadCommand(Guid Id, string Nombre, int Orden) : IRequest<ActividadDto>;
