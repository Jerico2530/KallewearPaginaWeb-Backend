using FluentValidation;

namespace BiblotecaWeb;

public class TestimonioDeleteValidacion : AbstractValidator<int>
{
    public TestimonioDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del testimonio debe ser válido para eliminar.");
    }
}
