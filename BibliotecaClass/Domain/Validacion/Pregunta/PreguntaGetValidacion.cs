using FluentValidation;

namespace BiblotecaWeb;

public class PreguntaGetValidacion : AbstractValidator<int>
{
    public PreguntaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del pregunta debe ser válido.");
    }
}
