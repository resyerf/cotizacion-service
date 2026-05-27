using ClosedXML.Excel;
using Cotizacion.Application.DTOs;
using Cotizacion.Application.Services;

namespace Cotizacion.Infrastructure.Services;

public sealed class ClosedXmlExcelService : IExcelService
{
    // ── Colour palette ────────────────────────────────────────────
    private static readonly XLColor ColDark       = XLColor.FromHtml("#0f172a");
    private static readonly XLColor ColBrand      = XLColor.FromHtml("#4f46e5");
    private static readonly XLColor ColBrandLight = XLColor.FromHtml("#eef2ff");
    private static readonly XLColor ColHeaderText = XLColor.FromHtml("#ffffff");
    private static readonly XLColor ColBorder     = XLColor.FromHtml("#cbd5e1");
    private static readonly XLColor ColBgAlt      = XLColor.FromHtml("#f8f9fb");
    private static readonly XLColor ColMuted      = XLColor.FromHtml("#64748b");
    private static readonly XLColor ColActBg      = XLColor.FromHtml("#1e293b");
    private static readonly XLColor ColActText    = XLColor.FromHtml("#e2e8f0");
    private static readonly XLColor ColTotalBg    = XLColor.FromHtml("#4f46e5");

    private static readonly Dictionary<int, string> MonedaLabel = new()
    {
        { 1, "USD" }, { 2, "PEN" }, { 3, "EUR" },
    };
    private static readonly Dictionary<int, string> MonedaPrefix = new()
    {
        { 1, "$ " }, { 2, "S/ " }, { 3, "€ " },
    };
    private static readonly Dictionary<int, string> EstadoLabel = new()
    {
        { 1, "Borrador" }, { 2, "Enviada" }, { 3, "En Revisión" },
        { 4, "Aprobada" }, { 5, "Rechazada" }, { 6, "Cancelada" },
    };

    // Column indices (1-based)
    private const int ColItem     = 1;
    private const int ColEnsayo   = 2;
    private const int ColUnidad   = 3;
    private const int ColCosto    = 4;
    private const int ColCantidad = 5;
    private const int ColSubtotal = 6;
    private const int LastCol     = 6;

    public byte[] GenerarCotizacion(CotizacionDetalleDto c)
    {
        using var wb = new XLWorkbook();
        wb.Style.Font.FontName = "Calibri";
        wb.Style.Font.FontSize = 10;

        var ws = wb.Worksheets.Add("Cotización");

        // Column widths
        ws.Column(ColItem).Width     = 10;
        ws.Column(ColEnsayo).Width   = 58;
        ws.Column(ColUnidad).Width   = 11;
        ws.Column(ColCosto).Width    = 14;
        ws.Column(ColCantidad).Width = 11;
        ws.Column(ColSubtotal).Width = 16;

        ws.ShowGridLines = false;

        var moneda  = (int)c.Moneda;
        var sym     = MonedaLabel.GetValueOrDefault(moneda, "USD");
        var prefix  = MonedaPrefix.GetValueOrDefault(moneda, "$ ");
        var estado  = EstadoLabel.GetValueOrDefault((int)c.Estado, c.Estado.ToString());

        int row = 1;

        // ── Header block ──────────────────────────────────────────
        row = DrawHeader(ws, c, sym, estado, row);

        // ── Column headers ────────────────────────────────────────
        row = DrawColumnHeaders(ws, sym, row);

        // ── Partidas grouped by activity ──────────────────────────
        var groups = c.Partidas
            .GroupBy(p => (p.ActividadOrden, p.ActividadNombre))
            .OrderBy(g => g.Key.ActividadOrden)
            .Select((g, idx) => (Index: idx + 1, Name: g.Key.ActividadNombre, Items: g.ToList()))
            .ToList();

        foreach (var (actIdx, actName, items) in groups)
        {
            var actSubtotal = items.Sum(p => p.Subtotal);
            row = DrawActivityRow(ws, actIdx, actName, actSubtotal, prefix, row);

            for (int j = 0; j < items.Count; j++)
                row = DrawItemRow(ws, $"{actIdx}.{j + 1:D2}", items[j], prefix, row, j);
        }

        // ── Totals ────────────────────────────────────────────────
        DrawTotalRow(ws, c.Total, prefix, row);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ── Header block ──────────────────────────────────────────────
    private static int DrawHeader(IXLWorksheet ws, CotizacionDetalleDto c, string sym, string estado, int row)
    {
        // Title bar (dark background)
        MergeFill(ws, row, 1, row, LastCol, ColDark);
        ws.Cell(row, ColItem).Value = "COTIZACIÓN";
        ws.Cell(row, ColItem).Style
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Font.SetFontSize(14)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        ws.Row(row).Height = 28;

        ws.Cell(row, ColSubtotal).Value = $"N° {c.Numero}";
        ws.Cell(row, ColSubtotal).Style
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Font.SetFontSize(13)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        row++;

        // Info rows
        void InfoRow(string label, string value, int r)
        {
            MergeFill(ws, r, 1, r, 2, XLColor.FromHtml("#1e293b"));
            ws.Cell(r, 1).Value = label;
            ws.Cell(r, 1).Style
                .Font.SetFontColor(ColActText)
                .Font.SetBold(true)
                .Font.SetFontSize(9)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            MergeFill(ws, r, 3, r, LastCol, XLColor.FromHtml("#0f172a"));
            ws.Cell(r, 3).Value = value;
            ws.Cell(r, 3).Style
                .Font.SetFontColor(XLColor.White)
                .Font.SetFontSize(9)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Row(r).Height = 17;
        }

        InfoRow("Cliente",  c.Cliente.Nombre, row++);
        InfoRow("Proyecto", c.Proyecto, row++);
        InfoRow("Fecha",    $"{FormatDate(c.Fecha.ToString())} — Estado: {estado}", row++);
        if (!string.IsNullOrWhiteSpace(c.Cliente.Ruc))
            InfoRow("RUC", c.Cliente.Ruc!, row++);
        if (!string.IsNullOrWhiteSpace(c.Notas))
            InfoRow("Notas", c.Notas!, row++);

        row++; // empty spacer
        return row;
    }

    // ── Column headers ────────────────────────────────────────────
    private static int DrawColumnHeaders(IXLWorksheet ws, string sym, int row)
    {
        string[] headers = ["ÍTEM", "ENSAYO", "UNIDAD", $"COSTO {sym}", "CANTIDAD", "SUBTOTAL"];

        for (int col = 1; col <= LastCol; col++)
        {
            var cell = ws.Cell(row, col);
            cell.Value = headers[col - 1];
            cell.Style
                .Fill.SetBackgroundColor(ColBrand)
                .Font.SetFontColor(ColHeaderText)
                .Font.SetBold(true)
                .Font.SetFontSize(9)
                .Alignment.SetHorizontal(col >= ColUnidad
                    ? XLAlignmentHorizontalValues.Center
                    : XLAlignmentHorizontalValues.Left)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(ColBrand);
        }

        ws.Cell(row, ColCosto).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        ws.Cell(row, ColSubtotal).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        ws.Row(row).Height = 20;
        return row + 1;
    }

    // ── Activity group header row (e.g. 1.00) ────────────────────
    private static int DrawActivityRow(
        IXLWorksheet ws, int actIdx, string actName, decimal subtotal, string prefix, int row)
    {
        // Ítem number
        ws.Cell(row, ColItem).Value = $"{actIdx}.00";
        ws.Cell(row, ColItem).Style
            .Fill.SetBackgroundColor(ColActBg)
            .Font.SetFontColor(ColActText)
            .Font.SetBold(true)
            .Font.SetFontSize(9)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetOutsideBorderColor(ColBorder);

        // Merged description across Ensayo → Cantidad
        var range = ws.Range(row, ColEnsayo, row, ColCantidad);
        range.Merge();
        range.Style
            .Fill.SetBackgroundColor(ColActBg)
            .Font.SetFontColor(ColActText)
            .Font.SetBold(true)
            .Font.SetFontSize(9)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetOutsideBorderColor(ColBorder);
        ws.Cell(row, ColEnsayo).Value = $"Actividad {actIdx}: {actName}";

        // Subtotal
        ws.Cell(row, ColSubtotal).Value = subtotal;
        ws.Cell(row, ColSubtotal).Style
            .Fill.SetBackgroundColor(ColActBg)
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Font.SetFontSize(9)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .NumberFormat.SetFormat($"\"{prefix}\"#,##0.00")
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetOutsideBorderColor(ColBorder);

        ws.Row(row).Height = 18;
        return row + 1;
    }

    // ── Item data row ─────────────────────────────────────────────
    private static int DrawItemRow(
        IXLWorksheet ws, string itemNum, CotizacionPartidaDto p, string prefix, int row, int idx)
    {
        var bg = idx % 2 == 0 ? XLColor.White : ColBgAlt;

        void Cell(int col, object? value, bool bold = false,
                  XLAlignmentHorizontalValues align = XLAlignmentHorizontalValues.Left,
                  string? fmt = null)
        {
            var c = ws.Cell(row, col);
            if (value is not null) c.Value = XLCellValue.FromObject(value);
            c.Style
                .Fill.SetBackgroundColor(bg)
                .Font.SetFontColor(ColDark)
                .Font.SetBold(bold)
                .Font.SetFontSize(9)
                .Alignment.SetHorizontal(align)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                .Alignment.SetWrapText(true)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(ColBorder);
            if (fmt is not null)
                c.Style.NumberFormat.SetFormat(fmt);
        }

        Cell(ColItem, itemNum);
        Cell(ColEnsayo, p.ItemDescripcion);
        Cell(ColUnidad, p.ItemUnidad, align: XLAlignmentHorizontalValues.Center);

        Cell(ColCosto, p.PrecioUnitario,
            align: XLAlignmentHorizontalValues.Right,
            fmt: $"\"{prefix}\"#,##0.00");

        if (p.Cantidad.HasValue)
            Cell(ColCantidad, (double)p.Cantidad.Value,
                align: XLAlignmentHorizontalValues.Center,
                fmt: "#,##0.##");
        else
            Cell(ColCantidad, null,
                align: XLAlignmentHorizontalValues.Center);

        if (p.Subtotal > 0)
            Cell(ColSubtotal, p.Subtotal, bold: true,
                align: XLAlignmentHorizontalValues.Right,
                fmt: $"\"{prefix}\"#,##0.00");
        else
        {
            // Show "$ -" for zero subtotals
            Cell(ColSubtotal, 0, bold: true,
                align: XLAlignmentHorizontalValues.Right,
                fmt: $"\"{prefix}\"#,##0.00;-\"{prefix}\"#,##0.00;\"{prefix}\"           -");
        }

        ws.Row(row).Height = 16;
        return row + 1;
    }

    // ── Grand total row ───────────────────────────────────────────
    private static void DrawTotalRow(IXLWorksheet ws, decimal total, string prefix, int row)
    {
        // Spacer row
        ws.Row(row).Height = 6;
        row++;

        // "Total Parcial" label (merge Ítem→Cantidad)
        var labelRange = ws.Range(row, ColItem, row, ColCantidad);
        labelRange.Merge();
        labelRange.Style
            .Fill.SetBackgroundColor(ColTotalBg)
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Font.SetFontSize(10)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetOutsideBorderColor(ColTotalBg);
        ws.Cell(row, ColItem).Value = "Total Parcial";

        // Total value
        var totalCell = ws.Cell(row, ColSubtotal);
        totalCell.Value = total;
        totalCell.Style
            .Fill.SetBackgroundColor(ColTotalBg)
            .Font.SetFontColor(XLColor.White)
            .Font.SetBold(true)
            .Font.SetFontSize(11)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right)
            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
            .NumberFormat.SetFormat($"\"{prefix}\"#,##0.00")
            .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
            .Border.SetOutsideBorderColor(ColTotalBg);

        ws.Row(row).Height = 22;
    }

    // ── Helpers ───────────────────────────────────────────────────
    private static void MergeFill(IXLWorksheet ws, int r1, int c1, int r2, int c2, XLColor bg)
    {
        var range = ws.Range(r1, c1, r2, c2);
        range.Merge();
        range.Style.Fill.SetBackgroundColor(bg);
    }

    private static string FormatDate(string iso)
    {
        if (DateOnly.TryParse(iso, out var d))
            return d.ToString("dd/MM/yyyy");
        return iso;
    }
}
