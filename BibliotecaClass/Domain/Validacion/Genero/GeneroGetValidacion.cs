using FluentValidation;

namespace BiblotecaWeb;

public class GeneroGetValidacion : AbstractValidator<int>
{
    public GeneroGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del genero debe ser válido.");
    }
}
