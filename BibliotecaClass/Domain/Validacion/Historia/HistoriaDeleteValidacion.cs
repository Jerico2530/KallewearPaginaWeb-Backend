using FluentValidation;

namespace BiblotecaWeb;

public class HistoriaDeleteValidacion : AbstractValidator<int>
{
    public HistoriaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del historia debe ser válido para eliminar.");
    }
}
