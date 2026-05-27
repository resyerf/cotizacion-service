using Cotizacion.Application.Commands.Clientes.ActualizarCliente;
using Cotizacion.Application.Commands.Clientes.CrearCliente;
using Cotizacion.Application.Commands.Clientes.DesactivarCliente;
using Cotizacion.Application.DTOs;
using Cotizacion.Application.Queries.Clientes.GetClientes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cotizacion.API.Controllers;

/// <summary>Gestión de clientes.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ClientesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todos los clientes registrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea un nuevo cliente.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearClienteCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    /// <summary>Actualiza los datos de un cliente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ActualizarClienteBody body, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ActualizarClienteCommand(id, body.Nombre, body.Ruc, body.Email, body.Telefono, body.Direccion),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>Desactiva un cliente (borrado lógico).</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DesactivarClienteCommand(id), cancellationToken);
        return NoContent();
    }
}

public record ActualizarClienteBody(
    string Nombre,
    string? Ruc,
    string? Email,
    string? Telefono,
    string? Direccion);
