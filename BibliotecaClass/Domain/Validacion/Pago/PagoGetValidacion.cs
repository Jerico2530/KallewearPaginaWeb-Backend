using FluentValidation;

namespace BiblotecaWeb;

public class PagoGetValidacion : AbstractValidator<int>
{
    public PagoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del pago debe ser válido.");
    }
}
