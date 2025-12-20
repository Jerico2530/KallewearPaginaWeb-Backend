using FluentValidation;

namespace BiblotecaWeb;

public class CategoriaDeleteValidacion : AbstractValidator<int>
{
    public CategoriaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del categoria debe ser válido para eliminar.");
    }
}
