using FluentValidation;

namespace Cotizacion.Application.Commands.Clientes.CrearCliente;

public sealed class CrearClienteCommandValidator : AbstractValidator<CrearClienteCommand>
{
    public CrearClienteCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(255).WithMessage("El nombre no puede exceder 255 caracteres.");

        RuleFor(x => x.Ruc)
            .MaximumLength(20).WithMessage("El RUC no puede exceder 20 caracteres.")
            .When(x => x.Ruc is not null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("El email no tiene formato válido.")
            .MaximumLength(255)
            .When(x => x.Email is not null);
    }
}
