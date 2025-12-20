using FluentValidation;

namespace BiblotecaWeb;

public class PermisoGetValidacion : AbstractValidator<int>
{
    public PermisoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del permiso debe ser válido.");
    }
}
