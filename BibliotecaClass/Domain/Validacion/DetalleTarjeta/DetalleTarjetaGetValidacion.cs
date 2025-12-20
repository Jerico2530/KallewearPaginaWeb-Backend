using FluentValidation;

namespace BiblotecaWeb;

public class DetalleTarjetaGetValidacion : AbstractValidator<int>
{
    public DetalleTarjetaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del detalle tarjeta debe ser válido.");
    }
}
