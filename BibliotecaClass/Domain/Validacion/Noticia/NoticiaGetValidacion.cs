using FluentValidation;

namespace BiblotecaWeb;

public class NoticiaGetValidacion : AbstractValidator<int>
{
    public NoticiaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del noticias debe ser válido.");
    }
}
