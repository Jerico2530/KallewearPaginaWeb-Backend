using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class DireccionUpdateValidacion : AbstractValidator<DireccionUpdateDto>
{
    public DireccionUpdateValidacion()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");

        RuleFor(x => x.Departamento)
            .NotEmpty().WithMessage("El Departamento  es obligatorio.")
            .MaximumLength(200).WithMessage("El Departamento no puede tener más de 200 caracteres.");

        RuleFor(x => x.Provincia)
           .NotEmpty().WithMessage("ElProvincia  es obligatorio.")
           .MaximumLength(200).WithMessage("El Provincia no puede tener más de 200 caracteres.");

        RuleFor(x => x.Distrito)
           .NotEmpty().WithMessage("El Distrito  es obligatorio.")
           .MaximumLength(200).WithMessage("El Distrito no puede tener más de 200 caracteres.");

        RuleFor(x => x.Via)
           .NotEmpty().WithMessage("El Via  es obligatorio.")
           .MaximumLength(200).WithMessage("El Via no puede tener más de 200 caracteres.");

        RuleFor(x => x.Numero)
           .NotEmpty().WithMessage("El Numero calle  es obligatorio.")
           .MaximumLength(200).WithMessage("El Numero calle no puede tener más de 200 caracteres.");
    }
}

