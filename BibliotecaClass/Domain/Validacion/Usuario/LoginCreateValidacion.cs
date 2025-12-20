using BiblotecaWeb.Domain.Dto.Usuario;
using FluentValidation;

namespace BiblotecaWeb;

public class LoginCreateValidacion : AbstractValidator<UsuarioLoginDto>
{
    public LoginCreateValidacion()
    {
        RuleFor(x => x.CorreoElectronico)
            .NotEmpty().WithMessage("El correo es obligatorio")
            .EmailAddress().WithMessage("Formato de correo inválido")
            .Matches(@"^[\w.+-]+@gmail\.com$").WithMessage("Solo se permiten correos @gmail.com");

        RuleFor(x => x.Contraseña)
            .NotEmpty().WithMessage("La contraseña es obligatoria");

    }
}
