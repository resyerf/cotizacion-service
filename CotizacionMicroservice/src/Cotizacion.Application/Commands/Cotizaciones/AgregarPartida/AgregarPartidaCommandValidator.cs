using FluentValidation;

namespace Cotizacion.Application.Commands.Cotizaciones.AgregarPartida;

public sealed class AgregarPartidaCommandValidator : AbstractValidator<AgregarPartidaCommand>
{
    public AgregarPartidaCommandValidator()
    {
        RuleFor(x => x.CotizacionId).NotEmpty();
        RuleFor(x => x.ItemCatalogoId).NotEmpty();
        RuleFor(x => x.PrecioUnitario).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Cantidad).GreaterThan(0).When(x => x.Cantidad.HasValue);
    }
}
