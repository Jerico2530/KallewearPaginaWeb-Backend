using FluentValidation;

namespace BiblotecaWeb;

public class MoneraDeleteValidacion : AbstractValidator<int>
{
    public MoneraDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del monera debe ser válido para eliminar.");
    }
}
