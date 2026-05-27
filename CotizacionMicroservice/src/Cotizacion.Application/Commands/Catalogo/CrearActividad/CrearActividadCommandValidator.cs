using FluentValidation;

namespace Cotizacion.Application.Commands.Catalogo.CrearActividad;

public sealed class CrearActividadCommandValidator : AbstractValidator<CrearActividadCommand>
{
    public CrearActividadCommandValidator()
    {
        RuleFor(x => x.Codigo).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Orden).GreaterThanOrEqualTo(0);
    }
}
