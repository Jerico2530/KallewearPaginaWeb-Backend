using BiblotecaWeb.Domain.Dto.Historia;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class HistoriaCreateValidacion : AbstractValidator<HistoriaCreateDto>
{
    public HistoriaCreateValidacion()
    {
        RuleFor(x => x.Año)
            .NotNull().WithMessage("Debe indicar la fecha de inicio.");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El titulo  es obligatorio.")
            .MaximumLength(100).WithMessage("El titulo no puede tener más de 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripcion es obligatorio.");
    }
}
