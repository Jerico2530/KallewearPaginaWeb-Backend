using BiblotecaWeb.Domain.Dto.Anuncio;
using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.Anuncio;

public class AnuncioUpdateValidacion : AbstractValidator<AnuncioUpdateDto>
{
    public AnuncioUpdateValidacion()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(100).WithMessage("El título no puede tener más de 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(500).WithMessage("La descripción no puede tener más de 500 caracteres.");

        RuleFor(x => x.Imagen)
            .NotEmpty().WithMessage("Debe seleccionar una imagen.")
            .MaximumLength(100).WithMessage("El nombre de la imagen no debe exceder 100 caracteres.");

        RuleFor(x => x.FechaInicio)
            .NotNull().WithMessage("Debe indicar la fecha de inicio.");

        RuleFor(x => x.FechaFinal)
            .NotNull().WithMessage("Debe indicar la fecha final.")
            .GreaterThan(x => x.FechaInicio)
            .WithMessage("La fecha final debe ser mayor que la fecha de inicio.");

        RuleFor(x => x.Orden)
            .GreaterThan(0).WithMessage("El orden debe ser mayor que 0.");
    }
}
