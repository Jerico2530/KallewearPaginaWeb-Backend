using BiblotecaWeb.Domain.Dto.Pregunta;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class PreguntaCreateValidacion : AbstractValidator<PreguntaCreateDto>
{
    public PreguntaCreateValidacion()
    {
        RuleFor(x => x.Preguntas)
           .NotEmpty().WithMessage("El Pregunta  es obligatorio.")
           .MaximumLength(400).WithMessage("El Pregunta no puede tener más de 400 caracteres.");

        RuleFor(x => x.Respuesta)
            .NotEmpty().WithMessage("El Respuesta  es obligatorio.")
            .MaximumLength(800).WithMessage("El Respuesta no puede tener más de 800 caracteres.");
    }
}
