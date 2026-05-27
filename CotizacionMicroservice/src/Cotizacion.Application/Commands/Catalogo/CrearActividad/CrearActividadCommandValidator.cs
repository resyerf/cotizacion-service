using FluentValidation;

namespace Cotizacion.Application.Commands.Catalogo.CrearActividad;

public sealed class CrearActividadCommandValidator : AbstractValidator<CrearActividadCommand>
{
    public CrearActividadCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(255);
    }
}
