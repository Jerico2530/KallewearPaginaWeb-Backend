using FluentValidation;

namespace BiblotecaWeb;

public class PreguntaDeleteValidacion : AbstractValidator<int>
{
    public PreguntaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del pregunta debe ser válido para eliminar.");
    }
}
