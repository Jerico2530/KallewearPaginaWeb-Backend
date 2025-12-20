using FluentValidation;

namespace BiblotecaWeb;

public class SucursalGetValidacion : AbstractValidator<int>
{
    public SucursalGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del sucursal debe ser válido.");
    }
}
