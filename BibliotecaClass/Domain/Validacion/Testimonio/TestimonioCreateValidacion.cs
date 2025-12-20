using BiblotecaWeb.Domain.Dto.Testimonio;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class TestimonioCreateValidacion : AbstractValidator<TestimonioCreateDto>
{
    public TestimonioCreateValidacion()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("El Descripcion  es obligatorio.");

        RuleFor(x => x.Evaluacion)
            .NotNull().WithMessage("La evaluación es obligatoria.")
           .InclusiveBetween(1, 5)
           .WithMessage("La evaluación debe estar entre 1 y 5.");
    }
}
