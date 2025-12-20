using FluentValidation;

namespace BiblotecaWeb;

public class HistoriaGetValidacion : AbstractValidator<int>
{
    public HistoriaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del historia debe ser válido.");
    }
}
