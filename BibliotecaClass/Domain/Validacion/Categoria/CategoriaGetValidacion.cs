using FluentValidation;

namespace BiblotecaWeb;

public class CategoriaGetValidacion : AbstractValidator<int>
{
    public CategoriaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del categoria debe ser válido.");
    }
}
