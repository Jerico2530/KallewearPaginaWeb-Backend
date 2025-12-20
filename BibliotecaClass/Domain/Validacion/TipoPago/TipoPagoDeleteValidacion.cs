using FluentValidation;

namespace BiblotecaWeb;

public class TipoPagoDeleteValidacion : AbstractValidator<int>
{
    public TipoPagoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del tipo-pago debe ser válido para eliminar.");
    }
}
