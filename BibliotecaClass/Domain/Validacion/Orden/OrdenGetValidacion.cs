using FluentValidation;

namespace BiblotecaWeb;

public class OrdenGetValidacion : AbstractValidator<int>
{
    public OrdenGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del orden debe ser válido.");
    }
}
