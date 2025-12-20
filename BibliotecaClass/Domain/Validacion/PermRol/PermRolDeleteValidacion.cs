using FluentValidation;

namespace BiblotecaWeb;

public class PermRolDeleteValidacion : AbstractValidator<int>
{
    public PermRolDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del permiso-rol debe ser válido para eliminar.");
    }
}
