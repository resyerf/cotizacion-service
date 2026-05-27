using Cotizacion.Domain.Interfaces.Repositories;
using Cotizacion.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cotizacion.Infrastructure.Persistence.Repositories;

public sealed class CotizacionRepository(CotizacionDbContext context)
    : Repository<Domain.Entities.Cotizacion, Guid>(context), ICotizacionRepository
{
    public async Task<Domain.Entities.Cotizacion?> GetByIdWithPartidasAsync(Guid id, CancellationToken cancellationToken = default) =>
        await Context.Cotizaciones
            .Include(c => c.Partidas)
                .ThenInclude(p => p.ItemCatalogo)
                    .ThenInclude(i => i!.Actividad)
            .Include(c => c.Cliente)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<bool> ExisteNumeroAsync(string numero, CancellationToken cancellationToken = default) =>
        await Context.Cotizaciones.AnyAsync(c => c.Numero == numero, cancellationToken);

    public async Task<IEnumerable<Domain.Entities.Cotizacion>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default) =>
        await Context.Cotizaciones.Where(c => c.ClienteId == clienteId).ToListAsync(cancellationToken);
}
