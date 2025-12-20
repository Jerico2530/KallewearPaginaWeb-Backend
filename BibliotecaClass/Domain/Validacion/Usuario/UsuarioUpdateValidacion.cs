using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class UsuarioUpdateValidacion : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateValidacion()
    {
        RuleFor(x => x.NombreCompleto)
            .MaximumLength(100).WithMessage("El Nombre Completo no puede tener más de 100 caracteres.");

        RuleFor(x => x.ApellidoCompleto)
 
            .MaximumLength(100).WithMessage("El Apellido Completo no puede tener más de 100 caracteres.");

        RuleFor(x => x.FechaNacimiento)
           .LessThanOrEqualTo(DateTime.Today.AddYears(-18))
           .WithMessage("El usuario debe tener al menos 18 años.");

        RuleFor(x => x.DNI)
            .MaximumLength(8).WithMessage("El DNI no puede tener más de 8 caracteres.");

        RuleFor(x => x.Imagen)
            .MaximumLength(100).WithMessage("El Numero Tarjeta no puede tener más de 100 url.");

        RuleFor(x => x.CorreoElectronico)
                 .MaximumLength(100).WithMessage("El correo electrónico no puede tener más de 100 caracteres.")
                 .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

    }
}
