using FluentValidation;

namespace BiblotecaWeb;

public class AnuncioDeleteValidacion : AbstractValidator<int>
{
    public AnuncioDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del anuncio debe ser válido para eliminar.");
    }
}
