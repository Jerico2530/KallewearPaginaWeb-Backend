using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class UsuarioCreateValidacion : AbstractValidator<UsuarioCreateDto>
{
    public UsuarioCreateValidacion()
    {
        RuleFor(x => x.NombreCompleto)
             .NotEmpty().WithMessage("El Nombre Completo  es obligatorio.")
             .MaximumLength(100).WithMessage("El Nombre Completo no puede tener más de 100 caracteres.");

        RuleFor(x => x.ApellidoCompleto)
            .NotEmpty().WithMessage("ElApellido Completo  es obligatorio.")
            .MaximumLength(100).WithMessage("El Apellido Completo no puede tener más de 100 caracteres.");

        RuleFor(x => x.FechaNacimiento)
           .NotEmpty().WithMessage("La fecha de nacimiento es obligatoria.")
           .LessThanOrEqualTo(DateTime.Today.AddYears(-18))
           .WithMessage("El usuario debe tener al menos 18 años.");

        RuleFor(x => x.DNI)
            .NotEmpty().WithMessage("El DNI  es obligatorio.")
            .MaximumLength(8).WithMessage("El DNI no puede tener más de 8 caracteres.");

        RuleFor(x => x.Imagen)
            .MaximumLength(100).WithMessage("El Numero Tarjeta no puede tener más de 100 url.");

        RuleFor(x => x.CorreoElectronico)
                 .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
                 .MaximumLength(100).WithMessage("El correo electrónico no puede tener más de 100 caracteres.")
                 .EmailAddress().WithMessage("El formato del correo electrónico no es válido.");

        RuleFor(x => x.Contraseña)
           .NotEmpty().WithMessage("El Contraseña  es obligatorio.")
            .MaximumLength(100).WithMessage("El Contraseña no puede tener más de 100 caracteres.");

        RuleFor(x => x.ContraseñaVisible)
            .NotEmpty().WithMessage("El Contraseña   es obligatorio.")
            .MaximumLength(100).WithMessage("El Contraseña  no puede tener más de 100 caracteres.");
    }
}
