using FluentValidation;

namespace BiblotecaWeb;

public class PermisoDeleteValidacion : AbstractValidator<int>
{
    public PermisoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del permiso debe ser válido para eliminar.");
    }
}
