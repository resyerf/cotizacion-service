using Cotizacion.Application.Common;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Clientes.DesactivarCliente;

public sealed class DesactivarClienteCommandHandler(
    IClienteRepository clienteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DesactivarClienteCommand>
{
    public async Task Handle(DesactivarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cliente), request.Id);

        cliente.Desactivar();
        clienteRepository.Update(cliente);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
