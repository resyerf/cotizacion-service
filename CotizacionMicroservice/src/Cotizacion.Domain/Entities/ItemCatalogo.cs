using Cotizacion.Domain.Common;
using Cotizacion.Domain.Enums;
using Cotizacion.Domain.Exceptions;

namespace Cotizacion.Domain.Entities;

public sealed class ItemCatalogo : AggregateRoot<Guid>
{
    public Guid ActividadId { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public string Unidad { get; private set; } = string.Empty;
    public decimal PrecioBase { get; private set; }
    public Moneda Moneda { get; private set; } = Moneda.USD;
    public bool Activo { get; private set; } = true;

    public Actividad? Actividad { get; private set; }

    private ItemCatalogo() { }

    public static ItemCatalogo Crear(
        Guid actividadId, string codigo, string descripcion,
        string unidad, decimal precioBase, Moneda moneda)
    {
        if (actividadId == Guid.Empty)
            throw new DomainException("La actividad es requerida.");
        if (string.IsNullOrWhiteSpace(codigo))
            throw new DomainException("El código del item es requerido.");
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción del item es requerida.");
        if (string.IsNullOrWhiteSpace(unidad))
            throw new DomainException("La unidad del item es requerida.");
        if (precioBase < 0)
            throw new DomainException("El precio base no puede ser negativo.");

        return new ItemCatalogo
        {
            Id = Guid.NewGuid(),
            ActividadId = actividadId,
            Codigo = codigo.Trim(),
            Descripcion = descripcion.Trim(),
            Unidad = unidad.Trim(),
            PrecioBase = precioBase,
            Moneda = moneda
        };
    }

    public void Actualizar(Guid actividadId, string descripcion, string unidad, decimal precioBase, Moneda moneda)
    {
        if (actividadId == Guid.Empty)
            throw new DomainException("La actividad es requerida.");
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción del item es requerida.");
        if (string.IsNullOrWhiteSpace(unidad))
            throw new DomainException("La unidad del item es requerida.");
        if (precioBase < 0)
            throw new DomainException("El precio no puede ser negativo.");

        ActividadId = actividadId;
        Descripcion = descripcion.Trim();
        Unidad = unidad.Trim();
        PrecioBase = precioBase;
        Moneda = moneda;
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activo = false;
        MarkUpdated();
    }
}
