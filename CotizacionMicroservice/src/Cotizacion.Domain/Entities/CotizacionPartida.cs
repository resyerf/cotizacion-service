using Cotizacion.Domain.Common;
using Cotizacion.Domain.Exceptions;

namespace Cotizacion.Domain.Entities;

public sealed class CotizacionPartida : Entity<Guid>
{
    public Guid CotizacionId { get; private set; }
    public Guid ItemCatalogoId { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal? Cantidad { get; private set; }

    public decimal Subtotal => Cantidad.HasValue
        ? Math.Round(PrecioUnitario * Cantidad.Value, 2)
        : 0m;

    public ItemCatalogo? ItemCatalogo { get; private set; }

    private CotizacionPartida() { }

    internal static CotizacionPartida Crear(Guid cotizacionId, Guid itemCatalogoId, decimal precioUnitario, decimal? cantidad)
    {
        if (precioUnitario < 0)
            throw new DomainException("El precio unitario no puede ser negativo.");
        if (cantidad.HasValue && cantidad.Value <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.");

        return new CotizacionPartida
        {
            Id = Guid.NewGuid(),
            CotizacionId = cotizacionId,
            ItemCatalogoId = itemCatalogoId,
            PrecioUnitario = precioUnitario,
            Cantidad = cantidad
        };
    }

    internal void ActualizarCantidad(decimal? cantidad)
    {
        if (cantidad.HasValue && cantidad.Value <= 0)
            throw new DomainException("La cantidad debe ser mayor a cero.");
        Cantidad = cantidad;
    }

    internal void ActualizarPrecio(decimal precio)
    {
        if (precio < 0)
            throw new DomainException("El precio no puede ser negativo.");
        PrecioUnitario = precio;
    }
}
