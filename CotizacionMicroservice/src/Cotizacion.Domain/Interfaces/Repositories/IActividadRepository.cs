using Cotizacion.Domain.Entities;

namespace Cotizacion.Domain.Interfaces.Repositories;

public interface IActividadRepository : IRepository<Actividad, Guid>
{
    Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Actividad>> GetActivasOrdenadasAsync(CancellationToken cancellationToken = default);
}
