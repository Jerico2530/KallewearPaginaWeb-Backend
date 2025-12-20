using FluentValidation;

namespace BiblotecaWeb;

public class TallaGetValidacion : AbstractValidator<int>
{
    public TallaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del talla debe ser válido.");
    }
}
