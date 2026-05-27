using Cotizacion.Application.DTOs;

namespace Cotizacion.Application.Services;

public interface IPdfService
{
    byte[] GenerarCotizacion(CotizacionDetalleDto cotizacion);
}
