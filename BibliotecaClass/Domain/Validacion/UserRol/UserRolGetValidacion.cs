using FluentValidation;

namespace BiblotecaWeb;

public class UserRolGetValidacion : AbstractValidator<int>
{
    public UserRolGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del user-rol debe ser válido.");
    }
}
