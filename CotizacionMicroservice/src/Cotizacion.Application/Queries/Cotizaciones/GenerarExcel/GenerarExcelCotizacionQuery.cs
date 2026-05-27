using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GenerarExcel;

public record GenerarExcelCotizacionQuery(Guid Id) : IRequest<ExcelResult>;

public record ExcelResult(byte[] Bytes, string Filename);
