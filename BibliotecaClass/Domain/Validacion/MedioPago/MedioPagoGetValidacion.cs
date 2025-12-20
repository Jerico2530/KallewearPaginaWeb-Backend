using FluentValidation;

namespace BiblotecaWeb;

public class MedioPagoGetValidacion : AbstractValidator<int>
{
    public MedioPagoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del medio pago debe ser válido.");
    }
}
