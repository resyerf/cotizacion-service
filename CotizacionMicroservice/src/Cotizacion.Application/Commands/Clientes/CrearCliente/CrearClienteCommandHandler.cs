using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Clientes.CrearCliente;

public sealed class CrearClienteCommandHandler(
    IClienteRepository clienteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearClienteCommand, ClienteDto>
{
    public async Task<ClienteDto> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
    {
        if (request.Ruc is not null && await clienteRepository.ExisteRucAsync(request.Ruc, cancellationToken: cancellationToken))
            throw new DomainException($"Ya existe un cliente con RUC '{request.Ruc}'.");

        var cliente = Cliente.Crear(request.Nombre, request.Ruc, request.Email, request.Telefono, request.Direccion);
        await clienteRepository.AddAsync(cliente, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClienteDto(cliente.Id, cliente.Nombre, cliente.Ruc, cliente.Email, cliente.Telefono, cliente.Direccion, cliente.Activo);
    }
}
