using FluentValidation;

namespace BiblotecaWeb;

public class SucursalDeleteValidacion : AbstractValidator<int>
{
    public SucursalDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del surcursal debe ser válido para eliminar.");
    }
}
