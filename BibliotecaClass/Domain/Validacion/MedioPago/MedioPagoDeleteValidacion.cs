using FluentValidation;

namespace BiblotecaWeb;

public class MedioPagoDeleteValidacion : AbstractValidator<int>
{
    public MedioPagoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del medio pago debe ser válido para eliminar.");
    }
}
