using Cotizacion.Application.Common;
using Cotizacion.Application.DTOs;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;
using CotizacionEntity = Cotizacion.Domain.Entities.Cotizacion;

namespace Cotizacion.Application.Commands.Cotizaciones.CrearCotizacion;

public sealed class CrearCotizacionCommandHandler(
    ICotizacionRepository cotizacionRepository,
    IClienteRepository clienteRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CrearCotizacionCommand, CotizacionResumenDto>
{
    public async Task<CotizacionResumenDto> Handle(CrearCotizacionCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.ClienteId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cliente), request.ClienteId);

        var numero = await GenerarNumeroAsync(cancellationToken);
        var cotizacion = CotizacionEntity.Crear(numero, request.ClienteId, request.Proyecto, request.Ubicacion,
            request.Fecha, request.FechaValidez, request.Moneda, request.Notas);

        await cotizacionRepository.AddAsync(cotizacion, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CotizacionResumenDto(cotizacion.Id, cotizacion.Numero, cotizacion.Proyecto, cotizacion.Ubicacion,
            cliente.Nombre, cotizacion.Fecha, cotizacion.FechaValidez, cotizacion.Estado, cotizacion.Moneda, cotizacion.Total);
    }

    private async Task<string> GenerarNumeroAsync(CancellationToken cancellationToken)
    {
        var anio = DateTime.UtcNow.Year;
        var secuencia = 1;

        while (true)
        {
            var numero = $"COT-{anio}-{secuencia:D4}";
            if (!await cotizacionRepository.ExisteNumeroAsync(numero, cancellationToken))
                return numero;
            secuencia++;
        }
    }
}
