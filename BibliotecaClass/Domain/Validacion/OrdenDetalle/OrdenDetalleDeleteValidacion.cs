using FluentValidation;

namespace BiblotecaWeb;

public class OrdenDetalleDeleteValidacion : AbstractValidator<int>
{
    public OrdenDetalleDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del orden detallado debe ser válido para eliminar.");
    }
}
