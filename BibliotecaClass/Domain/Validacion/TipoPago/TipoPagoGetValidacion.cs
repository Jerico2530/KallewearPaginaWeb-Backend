using FluentValidation;

namespace BiblotecaWeb;

public class TipoPagoGetValidacion : AbstractValidator<int>
{
    public TipoPagoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del tipo-pago debe ser válido.");
    }
}
