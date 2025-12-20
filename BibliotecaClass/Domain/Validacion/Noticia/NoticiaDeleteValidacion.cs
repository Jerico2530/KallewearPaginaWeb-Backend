using FluentValidation;

namespace BiblotecaWeb;

public class NoticiaDeleteValidacion : AbstractValidator<int>
{
    public NoticiaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del noticias debe ser válido para eliminar.");
    }
}
