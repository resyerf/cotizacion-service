using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Queries.Clientes.GetClientes;

public record GetClientesQuery : IRequest<IEnumerable<ClienteDto>>;
