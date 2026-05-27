using Cotizacion.Domain.Common;
using Cotizacion.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cotizacion.Infrastructure.Persistence.Context;

public sealed class CotizacionDbContext(DbContextOptions<CotizacionDbContext> options, IMediator mediator)
    : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Actividad> Actividades => Set<Actividad>();
    public DbSet<ItemCatalogo> ItemsCatalogo => Set<ItemCatalogo>();
    public DbSet<Domain.Entities.Cotizacion> Cotizaciones => Set<Domain.Entities.Cotizacion>();
    public DbSet<CotizacionPartida> CotizacionPartidas => Set<CotizacionPartida>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cotiz");
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CotizacionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregates = ChangeTracker.Entries<Entity<Guid>>()
            .Where(e => e.Entity.DomainEvents.Count != 0)
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();
        aggregates.ForEach(a => a.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await mediator.Publish(domainEvent, cancellationToken);
    }
}
