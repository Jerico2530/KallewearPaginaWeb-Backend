using BiblotecaWeb.Domain.Dto.Sucursal;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class SucursalUpdateValidacion : AbstractValidator<SucursalUpdateDto>
{
    public SucursalUpdateValidacion()
    {
        RuleFor(x => x.Locales)
             .NotEmpty().WithMessage("Los Locales  es obligatorio.")
             .MaximumLength(100).WithMessage("los Locales  no puede tener más de 100 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La Descripcion  es obligatorio.")
            .MaximumLength(200).WithMessage("La Descripcion no puede tener más de 200 caracteres.");
    }
}
