using Cotizacion.Application.Common;
using Cotizacion.Infrastructure.Persistence.Context;

namespace Cotizacion.Infrastructure.Persistence;

public sealed class UnitOfWork(CotizacionDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
