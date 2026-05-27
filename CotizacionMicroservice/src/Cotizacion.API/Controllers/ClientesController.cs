using Cotizacion.Application.Commands.Clientes.CrearCliente;
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
    /// <response code="200">Lista de clientes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetClientesQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Crea un nuevo cliente.</summary>
    /// <response code="201">Cliente creado exitosamente.</response>
    /// <response code="400">Datos de entrada inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CrearClienteCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }
}
