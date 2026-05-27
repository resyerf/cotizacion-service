using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Clientes.ActualizarCliente;

public sealed class ActualizarClienteCommandHandler(
    IClienteRepository clienteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ActualizarClienteCommand, ClienteDto>
{
    public async Task<ClienteDto> Handle(ActualizarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cliente), request.Id);

        cliente.Actualizar(request.Nombre, request.Ruc, request.Email, request.Telefono, request.Direccion);
        clienteRepository.Update(cliente);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClienteDto(cliente.Id, cliente.Nombre, cliente.Ruc,
            cliente.Email, cliente.Telefono, cliente.Direccion, cliente.Activo);
    }
}
