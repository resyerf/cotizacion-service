using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Interfaces.Repositories;
using Cotizacion.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cotizacion.Infrastructure.Persistence.Repositories;

public sealed class ItemCatalogoRepository(CotizacionDbContext context)
    : Repository<ItemCatalogo, Guid>(context), IItemCatalogoRepository
{
    public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        await Context.ItemsCatalogo.AnyAsync(i => i.Codigo == codigo && (excludeId == null || i.Id != excludeId), cancellationToken);

    public async Task<IEnumerable<ItemCatalogo>> GetByActividadIdAsync(Guid actividadId, CancellationToken cancellationToken = default) =>
        await Context.ItemsCatalogo
            .Include(i => i.Actividad)
            .Where(i => i.ActividadId == actividadId && i.Activo)
            .ToListAsync(cancellationToken);
}
