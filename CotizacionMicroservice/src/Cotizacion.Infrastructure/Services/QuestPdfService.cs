using Cotizacion.Application.DTOs;
using Cotizacion.Application.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace Cotizacion.Infrastructure.Services;

public sealed class QuestPdfService : IPdfService
{
    private const string ColBrand   = "#4f46e5";
    private const string ColDark    = "#0f172a";
    private const string ColMuted   = "#64748b";
    private const string ColBorder  = "#e2e8f0";
    private const string ColBgLight = "#f8f9fb";
    private const string ColBgCard  = "#ffffff";

    private static readonly Dictionary<int, string> EstadoColor = new()
    {
        { 1, "#6b7280" }, { 2, "#3b82f6" }, { 3, "#f59e0b" },
        { 4, "#10b981" }, { 5, "#ef4444" }, { 6, "#94a3b8" },
    };

    private static readonly Dictionary<int, string> EstadoLabel = new()
    {
        { 1, "Borrador" }, { 2, "Enviada" }, { 3, "En Revisión" },
        { 4, "Aprobada" }, { 5, "Rechazada" }, { 6, "Cancelada" },
    };

    private static readonly Dictionary<int, string> MonedaSym = new()
    {
        { 1, "USD" }, { 2, "PEN" }, { 3, "EUR" },
    };

    private static readonly Dictionary<int, string> MonedaPrefix = new()
    {
        { 1, "$ " }, { 2, "S/ " }, { 3, "€ " },
    };

    static QuestPdfService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerarCotizacion(CotizacionDetalleDto c)
    {
        var logo   = LoadLogo();
        var moneda = (int)c.Moneda;
        var sym    = MonedaSym.GetValueOrDefault(moneda, "USD");
        var prefix = MonedaPrefix.GetValueOrDefault(moneda, "$ ");

        return Document.Create(doc =>
        {
            doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(18, Unit.Millimetre);
                page.MarginVertical(14, Unit.Millimetre);
                page.DefaultTextStyle(t => t.FontFamily("Helvetica").FontSize(8).FontColor(ColDark));

                page.Header().Element(h => Header(h, c, logo, sym));
                page.Content().PaddingTop(12).Element(ct => Content(ct, c, prefix, sym));
                page.Footer().Element(f => Footer(f, c));
            });
        }).GeneratePdf();
    }

    // ── HEADER ────────────────────────────────────────────────────
    private static void Header(IContainer h, CotizacionDetalleDto c, byte[]? logo, string sym)
    {
        h.Background(ColDark).Padding(14).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                if (logo is { Length: > 0 })
                    col.Item().Width(45, Unit.Millimetre).Height(16, Unit.Millimetre)
                       .Image(logo).FitArea();
                else
                    col.Item().Text("COINGEC").FontSize(14).Bold().FontColor(Colors.White);

                col.Item().PaddingTop(6).Text(c.Proyecto).FontSize(11).Bold().FontColor(Colors.White);

                if (!string.IsNullOrWhiteSpace(c.Ubicacion))
                    col.Item().Text(c.Ubicacion).FontSize(8).FontColor("#94a3b8");
            });

            row.ConstantItem(90, Unit.Millimetre).Column(col =>
            {
                col.Item().AlignRight()
                   .Text("COTIZACIÓN").FontSize(7).FontColor("#94a3b8").LetterSpacing(0.08f);

                col.Item().PaddingTop(2).AlignRight()
                   .Text($"N° {c.Numero}").FontSize(18).Bold().FontColor(Colors.White);

                var label = EstadoLabel.GetValueOrDefault((int)c.Estado, c.Estado.ToString());
                var color = EstadoColor.GetValueOrDefault((int)c.Estado, "#6b7280");

                col.Item().PaddingTop(4).AlignRight()
                   .Background(color).Padding(3).PaddingHorizontal(8)
                   .Text(label).FontSize(7).Bold().FontColor(Colors.White);
            });
        });
    }

    // ── CONTENT ───────────────────────────────────────────────────
    private static void Content(IContainer ct, CotizacionDetalleDto c, string prefix, string sym)
    {
        ct.Column(col =>
        {
            // Info cards
            col.Item().Row(row =>
            {
                row.RelativeItem()
                   .Border(0.5f).BorderColor(ColBorder).Background(ColBgCard)
                   .Padding(10).Column(inner =>
                {
                    inner.Item().Text("CLIENTE")
                         .FontSize(7).Bold().FontColor(ColMuted).LetterSpacing(0.06f);
                    inner.Item().PaddingTop(4).Text(c.Cliente.Nombre).FontSize(10).Bold();

                    if (!string.IsNullOrWhiteSpace(c.Cliente.Ruc))
                        inner.Item().PaddingTop(2).Text($"RUC: {c.Cliente.Ruc}").FontSize(8).FontColor(ColMuted);
                    if (!string.IsNullOrWhiteSpace(c.Cliente.Email))
                        inner.Item().Text(c.Cliente.Email).FontSize(7.5f).FontColor(ColMuted);
                    if (!string.IsNullOrWhiteSpace(c.Cliente.Telefono))
                        inner.Item().Text(c.Cliente.Telefono).FontSize(7.5f).FontColor(ColMuted);
                    if (!string.IsNullOrWhiteSpace(c.Cliente.Direccion))
                        inner.Item().Text(c.Cliente.Direccion).FontSize(7.5f).FontColor(ColMuted);
                });

                row.ConstantItem(6);

                row.RelativeItem()
                   .Border(0.5f).BorderColor(ColBorder).Background(ColBgCard)
                   .Padding(10).Column(inner =>
                {
                    inner.Item().Text("FECHAS")
                         .FontSize(7).Bold().FontColor(ColMuted).LetterSpacing(0.06f);

                    inner.Item().PaddingTop(4).Row(r =>
                    {
                        r.ConstantItem(55).Text("Fecha de emisión:").FontSize(8).FontColor(ColMuted);
                        r.RelativeItem().Text(FormatDate(c.Fecha.ToString())).FontSize(8).Bold();
                    });

                    if (c.FechaValidez.HasValue)
                    {
                        inner.Item().PaddingTop(2).Row(r =>
                        {
                            r.ConstantItem(55).Text("Válida hasta:").FontSize(8).FontColor(ColMuted);
                            r.RelativeItem().Text(FormatDate(c.FechaValidez.Value.ToString())).FontSize(8).Bold();
                        });
                    }

                    inner.Item().PaddingTop(6).Text("MONEDA")
                         .FontSize(7).Bold().FontColor(ColMuted).LetterSpacing(0.06f);
                    inner.Item().PaddingTop(2).Text(sym).FontSize(9).Bold().FontColor(ColBrand);
                });
            });

            // Notes
            if (!string.IsNullOrWhiteSpace(c.Notas))
            {
                col.Item().PaddingTop(8)
                   .Border(0.5f).BorderColor("#fde68a").Background("#fefce8")
                   .Padding(10).Text(c.Notas).FontSize(8).FontColor("#92400e");
            }

            // Table title + accent line
            col.Item().PaddingTop(12).PaddingBottom(2)
               .Text("DETALLE DE PARTIDAS").FontSize(9).Bold().FontColor(ColDark);
            col.Item().PaddingBottom(6).Height(2).Background(ColBrand);

            // Table
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(22, Unit.Millimetre);  // ÍTEM
                    cols.RelativeColumn(3);                     // ENSAYO (flexible, wraps)
                    cols.ConstantColumn(14, Unit.Millimetre);  // UNIDAD
                    cols.ConstantColumn(28, Unit.Millimetre);  // COSTO
                    cols.ConstantColumn(16, Unit.Millimetre);  // CANTIDAD
                    cols.ConstantColumn(24, Unit.Millimetre);  // SUB TOTAL
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle()
                          .Text("ÍTEM").FontSize(7).Bold().FontColor(Colors.White);

                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle()
                          .Text("ENSAYO").FontSize(7).Bold().FontColor(Colors.White);

                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle().AlignCenter()
                          .Text("UNIDAD").FontSize(7).Bold().FontColor(Colors.White);

                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle().AlignRight()
                          .Text($"COSTO {sym}").FontSize(7).Bold().FontColor(Colors.White);

                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle().AlignRight()
                          .Text("CANTIDAD").FontSize(7).Bold().FontColor(Colors.White);

                    header.Cell().Background(ColDark).Padding(6).PaddingVertical(7).AlignMiddle().AlignRight()
                          .Text("SUB TOTAL").FontSize(7).Bold().FontColor(Colors.White);
                });

                // Rows
                for (int i = 0; i < c.Partidas.Count; i++)
                {
                    var p  = c.Partidas[i];
                    var bg = i % 2 == 0 ? ColBgLight : ColBgCard;

                    // ÍTEM
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle()
                         .Text(p.ItemCodigo).FontFamily("Courier New").FontSize(7).Bold().FontColor(ColBrand);

                    // ENSAYO — wraps automatically, grows row height
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle().Column(inner =>
                    {
                        inner.Item().Text(p.ItemDescripcion).FontSize(8);
                        if (!string.IsNullOrWhiteSpace(p.ActividadNombre))
                            inner.Item().PaddingTop(1)
                                 .Text(p.ActividadNombre).FontSize(6.5f).FontColor(ColMuted);
                    });

                    // UNIDAD
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle().AlignCenter()
                         .Text(p.ItemUnidad).FontSize(8);

                    // COSTO
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle().AlignRight()
                         .Text(FormatMoney(p.PrecioUnitario, prefix)).FontSize(8);

                    // CANTIDAD
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle().AlignRight()
                         .Text(p.Cantidad.HasValue ? p.Cantidad.Value.ToString("G") : "—").FontSize(8);

                    // SUB TOTAL
                    table.Cell().Background(bg).BorderBottom(0.3f).BorderColor(ColBorder)
                         .Padding(5).PaddingVertical(6).AlignMiddle().AlignRight()
                         .Text(FormatMoney(p.Subtotal, prefix)).FontSize(8).Bold();
                }

                // Total
                table.Footer(footer =>
                {
                    footer.Cell().ColumnSpan(5)
                          .Background(ColBrand).Padding(8).PaddingVertical(9).AlignMiddle()
                          .Text("TOTAL").FontSize(9).Bold().FontColor(Colors.White);

                    footer.Cell()
                          .Background(ColBrand).Padding(8).PaddingVertical(9).AlignMiddle().AlignRight()
                          .Text(FormatMoney(c.Total, prefix)).FontSize(10).Bold().FontColor(Colors.White);
                });
            });
        });
    }

    // ── FOOTER ────────────────────────────────────────────────────
    private static void Footer(IContainer f, CotizacionDetalleDto c)
    {
        f.BorderTop(0.3f).BorderColor(ColBorder).PaddingTop(6).Row(row =>
        {
            row.RelativeItem()
               .Text($"Cotización N° {c.Numero} — Generada el {DateOnly.FromDateTime(DateTime.Today):dd/MM/yyyy}")
               .FontSize(7).FontColor(ColMuted);

            row.ConstantItem(130).AlignRight()
               .Text("Documento generado digitalmente").FontSize(7).FontColor(ColMuted);
        });
    }

    // ── HELPERS ───────────────────────────────────────────────────
    private static string FormatMoney(decimal value, string prefix) =>
        $"{prefix}{value:N2}";

    private static string FormatDate(string iso)
    {
        if (DateOnly.TryParse(iso, out var d))
            return d.ToString("dd/MM/yyyy");
        return iso;
    }

    private static byte[]? LoadLogo()
    {
        var assembly = typeof(QuestPdfService).Assembly;
        var name = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("coingec-logo.png", StringComparison.OrdinalIgnoreCase));

        if (name is null) return null;

        using var stream = assembly.GetManifestResourceStream(name);
        if (stream is null) return null;

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }
}
