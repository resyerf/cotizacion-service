using Cotizacion.Domain.Entities;

namespace Cotizacion.Domain.Interfaces.Repositories;

public interface IItemCatalogoRepository : IRepository<ItemCatalogo, Guid>
{
    Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItemCatalogo>> GetByActividadIdAsync(Guid actividadId, CancellationToken cancellationToken = default);
}
