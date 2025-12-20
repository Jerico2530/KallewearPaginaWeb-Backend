using FluentValidation;

namespace BiblotecaWeb;

public class MoneraGetValidacion : AbstractValidator<int>
{
    public MoneraGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del monera debe ser válido.");
    }
}
