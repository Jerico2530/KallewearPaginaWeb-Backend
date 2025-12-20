using FluentValidation;

namespace BiblotecaWeb;

public class DireccionGetValidacion : AbstractValidator<int>
{
    public DireccionGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del direccion debe ser válido.");
    }
}
