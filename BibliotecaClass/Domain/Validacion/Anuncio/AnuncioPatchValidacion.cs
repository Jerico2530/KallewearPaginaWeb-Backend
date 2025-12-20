using BiblotecaWeb.Domain.Dto.Anuncio;
using FluentValidation;

namespace BiblotecaWeb;

public class AnuncioPatchValidacion : AbstractValidator<AnuncioUpdateDto>
{
    public AnuncioPatchValidacion()
    {
        When(x => x.Titulo != null, () =>
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El título no puede estar vacío si se incluye.")
                .MaximumLength(100).WithMessage("El título no puede tener más de 100 caracteres.");
        });

        When(x => x.Descripcion != null, () =>
        {
            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("La descripción no puede estar vacía si se incluye.")
                .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres.");
        });

        When(x => x.Imagen != null, () =>
        {
            RuleFor(x => x.Imagen)
                .NotEmpty().WithMessage("Debe incluir una imagen válida.")
                .MaximumLength(100).WithMessage("El nombre de la imagen no debe exceder 100 caracteres.");
        });

        When(x => x.FechaInicio != default, () =>
        {
            RuleFor(x => x.FechaFinal)
                .GreaterThan(x => x.FechaInicio)
                .WithMessage("La fecha final debe ser mayor que la fecha de inicio.");
        });
    }
}
