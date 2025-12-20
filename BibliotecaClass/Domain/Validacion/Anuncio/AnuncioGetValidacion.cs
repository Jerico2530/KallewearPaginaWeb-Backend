using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.Anuncio;

public class AnuncioGetValidacion : AbstractValidator<int>
{
    public AnuncioGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del anuncio debe ser válido.");
    }
}
