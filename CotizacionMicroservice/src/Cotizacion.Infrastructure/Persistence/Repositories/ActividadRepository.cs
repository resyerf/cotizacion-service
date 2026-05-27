using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Interfaces.Repositories;
using Cotizacion.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cotizacion.Infrastructure.Persistence.Repositories;

public sealed class ActividadRepository(CotizacionDbContext context)
    : Repository<Actividad, Guid>(context), IActividadRepository
{
    public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        await Context.Actividades.AnyAsync(a => a.Codigo == codigo.ToUpperInvariant() && (excludeId == null || a.Id != excludeId), cancellationToken);

    public async Task<IEnumerable<Actividad>> GetActivasOrdenadasAsync(CancellationToken cancellationToken = default) =>
        await Context.Actividades.Where(a => a.Activo).OrderBy(a => a.Orden).ToListAsync(cancellationToken);
}
