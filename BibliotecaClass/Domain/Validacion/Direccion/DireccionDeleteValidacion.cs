using FluentValidation;

namespace BiblotecaWeb;

public class DireccionDeleteValidacion : AbstractValidator<int>
{
    public DireccionDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del direccion debe ser válido para eliminar.");
    }
}

