using Cotizacion.Domain.Entities;

namespace Cotizacion.Domain.Interfaces.Repositories;

public interface IClienteRepository : IRepository<Cliente, Guid>
{
    Task<Cliente?> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    Task<bool> ExisteRucAsync(string ruc, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
