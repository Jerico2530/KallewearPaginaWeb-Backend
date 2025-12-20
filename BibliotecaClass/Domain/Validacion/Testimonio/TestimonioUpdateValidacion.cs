using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class TestimonioUpdateValidacion : AbstractValidator<TestimonioUpdateDto>
{
    public TestimonioUpdateValidacion()
    {

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("El Descripcion  es obligatorio.");

        RuleFor(x => x.Evaluacion)
            .NotNull().WithMessage("La evaluación es obligatoria.")
           .InclusiveBetween(1, 5)
           .WithMessage("La evaluación debe estar entre 1 y 5.");
    }
}
