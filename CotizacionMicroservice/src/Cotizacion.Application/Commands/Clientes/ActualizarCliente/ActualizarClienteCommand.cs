using Cotizacion.Application.DTOs;
using MediatR;

namespace Cotizacion.Application.Commands.Clientes.ActualizarCliente;

public record ActualizarClienteCommand(
    Guid Id,
    string Nombre,
    string? Ruc,
    string? Email,
    string? Telefono,
    string? Direccion) : IRequest<ClienteDto>;
