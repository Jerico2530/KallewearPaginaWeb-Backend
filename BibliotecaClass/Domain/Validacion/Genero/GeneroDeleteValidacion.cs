using FluentValidation;

namespace BiblotecaWeb;

public class GeneroDeleteValidacion : AbstractValidator<int>
{
    public GeneroDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del genero debe ser válido para eliminar.");
    }
}
