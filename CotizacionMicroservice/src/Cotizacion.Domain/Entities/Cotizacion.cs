using Cotizacion.Domain.Common;
using Cotizacion.Domain.Enums;
using Cotizacion.Domain.Events;
using Cotizacion.Domain.Exceptions;

namespace Cotizacion.Domain.Entities;

public sealed class Cotizacion : AggregateRoot<Guid>
{
    public string Numero { get; private set; } = string.Empty;
    public Guid ClienteId { get; private set; }
    public string Proyecto { get; private set; } = string.Empty;
    public string? Ubicacion { get; private set; }
    public DateOnly Fecha { get; private set; }
    public DateOnly? FechaValidez { get; private set; }
    public EstadoCotizacion Estado { get; private set; } = EstadoCotizacion.Borrador;
    public Moneda Moneda { get; private set; } = Moneda.USD;
    public string? Notas { get; private set; }

    public decimal Total => _partidas.Sum(p => p.Subtotal);

    public Cliente? Cliente { get; private set; }

    private readonly List<CotizacionPartida> _partidas = [];
    public IReadOnlyCollection<CotizacionPartida> Partidas => _partidas.AsReadOnly();

    private Cotizacion() { }

    public static Cotizacion Crear(string numero, Guid clienteId, string proyecto, string? ubicacion,
        DateOnly fecha, DateOnly? fechaValidez, Moneda moneda, string? notas)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("El número de cotización es requerido.");
        if (clienteId == Guid.Empty)
            throw new DomainException("El cliente es requerido.");
        if (string.IsNullOrWhiteSpace(proyecto))
            throw new DomainException("El nombre del proyecto es requerido.");
        if (fechaValidez.HasValue && fechaValidez.Value < fecha)
            throw new DomainException("La fecha de validez no puede ser anterior a la fecha de cotización.");

        var cotizacion = new Cotizacion
        {
            Id = Guid.NewGuid(),
            Numero = numero.Trim(),
            ClienteId = clienteId,
            Proyecto = proyecto.Trim(),
            Ubicacion = ubicacion?.Trim(),
            Fecha = fecha,
            FechaValidez = fechaValidez,
            Moneda = moneda,
            Notas = notas?.Trim()
        };

        cotizacion.RaiseDomainEvent(new CotizacionCreadaEvent(cotizacion.Id, cotizacion.Numero, clienteId));
        return cotizacion;
    }

    public void AgregarPartida(Guid itemCatalogoId, decimal precioUnitario, decimal? cantidad)
    {
        EnsureEditable();

        if (_partidas.Any(p => p.ItemCatalogoId == itemCatalogoId))
            throw new DomainException("El item ya existe en la cotización.");

        _partidas.Add(CotizacionPartida.Crear(Id, itemCatalogoId, precioUnitario, cantidad));
        MarkUpdated();
    }

    public void ActualizarPartida(Guid itemCatalogoId, decimal? cantidad, decimal? precio)
    {
        EnsureEditable();

        var partida = _partidas.FirstOrDefault(p => p.ItemCatalogoId == itemCatalogoId)
            ?? throw new DomainException($"El item no existe en esta cotización.");

        if (cantidad.HasValue) partida.ActualizarCantidad(cantidad.Value);
        if (precio.HasValue) partida.ActualizarPrecio(precio.Value);
        MarkUpdated();
    }

    public void RemoverPartida(Guid itemCatalogoId)
    {
        EnsureEditable();
        var partida = _partidas.FirstOrDefault(p => p.ItemCatalogoId == itemCatalogoId)
            ?? throw new DomainException("El item no existe en esta cotización.");
        _partidas.Remove(partida);
        MarkUpdated();
    }

    public void CambiarEstado(EstadoCotizacion nuevoEstado)
    {
        ValidarTransicionEstado(nuevoEstado);
        var estadoAnterior = Estado;
        Estado = nuevoEstado;
        MarkUpdated();
        RaiseDomainEvent(new CotizacionEstadoCambiadoEvent(Id, estadoAnterior, nuevoEstado));
    }

    public void ActualizarNotas(string? notas)
    {
        EnsureEditable();
        Notas = notas?.Trim();
        MarkUpdated();
    }

    private void EnsureEditable()
    {
        if (Estado is EstadoCotizacion.Aprobada or EstadoCotizacion.Cancelada)
            throw new DomainException($"No se puede modificar una cotización en estado '{Estado}'.");
    }

    private void ValidarTransicionEstado(EstadoCotizacion nuevo)
    {
        var transicionesValidas = new Dictionary<EstadoCotizacion, IEnumerable<EstadoCotizacion>>
        {
            [EstadoCotizacion.Borrador]    = [EstadoCotizacion.Enviada, EstadoCotizacion.Cancelada],
            [EstadoCotizacion.Enviada]     = [EstadoCotizacion.EnRevision, EstadoCotizacion.Aprobada, EstadoCotizacion.Rechazada, EstadoCotizacion.Cancelada],
            [EstadoCotizacion.EnRevision]  = [EstadoCotizacion.Aprobada, EstadoCotizacion.Rechazada, EstadoCotizacion.Cancelada],
            [EstadoCotizacion.Rechazada]   = [EstadoCotizacion.Borrador, EstadoCotizacion.Cancelada],
            [EstadoCotizacion.Aprobada]    = [],
            [EstadoCotizacion.Cancelada]   = []
        };

        if (!transicionesValidas[Estado].Contains(nuevo))
            throw new DomainException($"No se puede pasar de '{Estado}' a '{nuevo}'.");
    }
}
