using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class MoneraUpdateValidacion : AbstractValidator<MonedaUpdateDto>
{
    public MoneraUpdateValidacion()
    {
        RuleFor(x => x.Codigo)
             .NotEmpty().WithMessage("El Codigo  es obligatorio.")
             .MaximumLength(5).WithMessage("El Codigo no puede tener más de 5 caracteres.");

        RuleFor(x => x.Nombre)
             .NotEmpty().WithMessage("El Nombre de la monera   es obligatorio.")
             .MaximumLength(20).WithMessage("El Nombre de la monera no puede tener más de 20 caracteres.");

        RuleFor(x => x.Simbolo)
            .NotEmpty().WithMessage("El Simbolo  es obligatorio.")
            .MaximumLength(5).WithMessage("El Simbolo no puede tener más de 5 caracteres.");
    }
}
