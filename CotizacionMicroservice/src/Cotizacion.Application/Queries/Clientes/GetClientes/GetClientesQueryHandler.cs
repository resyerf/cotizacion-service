using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Queries.Clientes.GetClientes;

public sealed class GetClientesQueryHandler(IClienteRepository clienteRepository)
    : IRequestHandler<GetClientesQuery, IEnumerable<ClienteDto>>
{
    public async Task<IEnumerable<ClienteDto>> Handle(GetClientesQuery request, CancellationToken cancellationToken)
    {
        var clientes = await clienteRepository.FindAsync(c => c.Activo, cancellationToken);
        return clientes.Select(c => new ClienteDto(c.Id, c.Nombre, c.Ruc, c.Email, c.Telefono, c.Direccion, c.Activo));
    }
}
