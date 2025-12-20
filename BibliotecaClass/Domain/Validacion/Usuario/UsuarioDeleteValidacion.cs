using FluentValidation;

namespace BiblotecaWeb;

public class UsuarioDeleteValidacion : AbstractValidator<int>
{
    public UsuarioDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del usuario debe ser válido para eliminar.");
    }
}