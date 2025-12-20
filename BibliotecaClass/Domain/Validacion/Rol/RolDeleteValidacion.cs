using FluentValidation;

namespace BiblotecaWeb;

public class RolDeleteValidacion : AbstractValidator<int>
{
    public RolDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del rol debe ser válido para eliminar.");
    }
}
