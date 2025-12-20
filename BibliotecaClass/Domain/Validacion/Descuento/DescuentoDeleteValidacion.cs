using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.Descuento;

public class DescuentoDeleteValidacion : AbstractValidator<int>
{
    public DescuentoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del descuento  debe ser válido para eliminar.");
    }
}
