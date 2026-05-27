using Cotizacion.Application.DTOs;

namespace Cotizacion.Application.Services;

public interface IExcelService
{
    byte[] GenerarCotizacion(CotizacionDetalleDto cotizacion);
}
