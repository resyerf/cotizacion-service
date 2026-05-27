using MediatR;

namespace Cotizacion.Application.Queries.Cotizaciones.GenerarPdf;

public record GenerarPdfCotizacionQuery(Guid Id) : IRequest<PdfResult>;

public record PdfResult(byte[] Bytes, string Filename);
