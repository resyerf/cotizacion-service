using FluentValidation;

namespace Cotizacion.Application.Commands.Catalogo.CrearItemCatalogo;

public sealed class CrearItemCatalogoCommandValidator : AbstractValidator<CrearItemCatalogoCommand>
{
    public CrearItemCatalogoCommandValidator()
    {
        RuleFor(x => x.ActividadId).NotEmpty();
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Unidad).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PrecioBase).GreaterThanOrEqualTo(0);
    }
}
