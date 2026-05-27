using MediatR;

namespace Cotizacion.Application.Commands.Clientes.DesactivarCliente;

public record DesactivarClienteCommand(Guid Id) : IRequest;
