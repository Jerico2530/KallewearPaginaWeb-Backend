using FluentValidation;

namespace BiblotecaWeb;

public class UserRolDeleteValidacion : AbstractValidator<int>
{
    public UserRolDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del user-rol debe ser válido para eliminar.");
    }
}
