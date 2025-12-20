using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.Descuento;

public class DescuentoGetValidacion : AbstractValidator<int>
{
    public DescuentoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del descuento debe ser válido.");
    }
}
