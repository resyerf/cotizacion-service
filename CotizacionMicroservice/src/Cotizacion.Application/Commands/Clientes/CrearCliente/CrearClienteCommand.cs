using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Commands.Clientes.CrearCliente;

public record CrearClienteCommand(
    string Nombre,
    string? Ruc,
    string? Email,
    string? Telefono,
    string? Direccion) : IRequest<ClienteDto>;
