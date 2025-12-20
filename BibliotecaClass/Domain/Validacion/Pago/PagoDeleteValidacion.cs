using FluentValidation;

namespace BiblotecaWeb;

public class PagoDeleteValidacion : AbstractValidator<int>
{
    public PagoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del pago debe ser válido para eliminar.");
    }
}
