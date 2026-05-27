using FluentValidation;

namespace Cotizacion.Application.Commands.Cotizaciones.CrearCotizacion;

public sealed class CrearCotizacionCommandValidator : AbstractValidator<CrearCotizacionCommand>
{
    public CrearCotizacionCommandValidator()
    {
        RuleFor(x => x.ClienteId).NotEmpty().WithMessage("El cliente es requerido.");
        RuleFor(x => x.Proyecto).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Ubicacion).MaximumLength(255).When(x => x.Ubicacion is not null);
        RuleFor(x => x.Fecha).NotEmpty();
        RuleFor(x => x.FechaValidez)
            .GreaterThanOrEqualTo(x => x.Fecha)
            .WithMessage("La fecha de validez debe ser posterior a la fecha de cotización.")
            .When(x => x.FechaValidez.HasValue);
        RuleFor(x => x.Moneda).IsInEnum();
    }
}
