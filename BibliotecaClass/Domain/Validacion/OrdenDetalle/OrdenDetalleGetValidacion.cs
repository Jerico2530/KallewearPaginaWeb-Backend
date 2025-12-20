using FluentValidation;

namespace BiblotecaWeb;

public class OrdenDetalleGetValidacion : AbstractValidator<int>
{
    public OrdenDetalleGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del orden detallado debe ser válido.");
    }
}
