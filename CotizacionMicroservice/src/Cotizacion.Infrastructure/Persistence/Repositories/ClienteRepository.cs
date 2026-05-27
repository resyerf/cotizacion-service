using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Interfaces.Repositories;
using Cotizacion.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Cotizacion.Infrastructure.Persistence.Repositories;

public sealed class ClienteRepository(CotizacionDbContext context)
    : Repository<Cliente, Guid>(context), IClienteRepository
{
    public async Task<Cliente?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default) =>
        await Context.Clientes.FirstOrDefaultAsync(c => c.Ruc == ruc, cancellationToken);

    public async Task<bool> ExisteRucAsync(string ruc, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        await Context.Clientes.AnyAsync(c => c.Ruc == ruc && (excludeId == null || c.Id != excludeId), cancellationToken);
}
