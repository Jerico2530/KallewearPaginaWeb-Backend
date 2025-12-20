using FluentValidation;

namespace BiblotecaWeb;

public class TestimonioGetValidacion : AbstractValidator<int>
{
    public TestimonioGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del testimonio debe ser válido.");
    }
}
