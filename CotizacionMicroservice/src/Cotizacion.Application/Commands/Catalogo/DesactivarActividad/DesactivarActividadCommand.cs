using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.DesactivarActividad;

public record DesactivarActividadCommand(Guid Id) : IRequest;
