using CotizacionEntity = Cotizacion.Domain.Entities.Cotizacion;

namespace Cotizacion.Domain.Interfaces.Repositories;

public interface ICotizacionRepository : IRepository<CotizacionEntity, Guid>
{
    Task<CotizacionEntity?> GetByIdWithPartidasAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExisteNumeroAsync(string numero, CancellationToken cancellationToken = default);
    Task<IEnumerable<CotizacionEntity>> GetByClienteIdAsync(Guid clienteId, CancellationToken cancellationToken = default);
}
