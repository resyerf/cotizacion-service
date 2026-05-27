using Cotizacion.Application.Common;
using Cotizacion.Domain.Entities;
using Cotizacion.Domain.Exceptions;
using Cotizacion.Domain.Interfaces.Repositories;
using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.DesactivarItemCatalogo;

public sealed class DesactivarItemCatalogoCommandHandler(
    IItemCatalogoRepository itemRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DesactivarItemCatalogoCommand>
{
    public async Task Handle(DesactivarItemCatalogoCommand request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(ItemCatalogo), request.Id);

        item.Desactivar();
        itemRepository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
