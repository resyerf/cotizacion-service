using Cotizacion.Domain.Common;
using Cotizacion.Domain.Exceptions;

namespace Cotizacion.Domain.Entities;

public sealed class Actividad : AggregateRoot<Guid>
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public int Orden { get; private set; }
    public bool Activo { get; private set; } = true;

    private readonly List<ItemCatalogo> _items = [];
    public IReadOnlyCollection<ItemCatalogo> Items => _items.AsReadOnly();

    private Actividad() { }

    public static Actividad Crear(string codigo, string nombre, int orden)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new DomainException("El código de actividad es requerido.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de actividad es requerido.");

        return new Actividad
        {
            Id = Guid.NewGuid(),
            Codigo = codigo.Trim().ToUpperInvariant(),
            Nombre = nombre.Trim(),
            Orden = orden
        };
    }

    public void Actualizar(string nombre, int orden)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de actividad es requerido.");

        Nombre = nombre.Trim();
        Orden = orden;
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activo = false;
        MarkUpdated();
    }
}
