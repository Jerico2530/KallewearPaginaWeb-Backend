using FluentValidation;

namespace BiblotecaWeb;

public class DetalleTarjetaDeleteValidacion : AbstractValidator<int>
{
    public DetalleTarjetaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del detalle tarjeta debe ser válido para eliminar.");
    }
}
