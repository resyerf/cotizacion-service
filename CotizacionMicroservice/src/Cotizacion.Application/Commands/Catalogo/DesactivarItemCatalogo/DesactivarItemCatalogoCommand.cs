using MediatR;

namespace Cotizacion.Application.Commands.Catalogo.DesactivarItemCatalogo;

public record DesactivarItemCatalogoCommand(Guid Id) : IRequest;
