using Cotizacion.Domain.Common;
using Cotizacion.Domain.Exceptions;

namespace Cotizacion.Domain.Entities;

public sealed class Cliente : AggregateRoot<Guid>
{
    public string Nombre { get; private set; } = string.Empty;
    public string? Ruc { get; private set; }
    public string? Email { get; private set; }
    public string? Telefono { get; private set; }
    public string? Direccion { get; private set; }
    public bool Activo { get; private set; } = true;

    private Cliente() { }

    public static Cliente Crear(string nombre, string? ruc, string? email, string? telefono, string? direccion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del cliente es requerido.");

        return new Cliente
        {
            Id = Guid.NewGuid(),
            Nombre = nombre.Trim(),
            Ruc = ruc?.Trim(),
            Email = email?.Trim(),
            Telefono = telefono?.Trim(),
            Direccion = direccion?.Trim()
        };
    }

    public void Actualizar(string nombre, string? ruc, string? email, string? telefono, string? direccion)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del cliente es requerido.");

        Nombre = nombre.Trim();
        Ruc = ruc?.Trim();
        Email = email?.Trim();
        Telefono = telefono?.Trim();
        Direccion = direccion?.Trim();
        MarkUpdated();
    }

    public void Desactivar()
    {
        Activo = false;
        MarkUpdated();
    }
}
