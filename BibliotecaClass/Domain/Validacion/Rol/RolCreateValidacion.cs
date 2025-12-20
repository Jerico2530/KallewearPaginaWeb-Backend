using BiblotecaWeb.Domain.Dto.Rol;
using FluentValidation;

namespace BiblotecaWeb;

public class RolCreateValidacion : AbstractValidator<RolCreateDto>
{
    public RolCreateValidacion()
    {
        RuleFor(x => x.NombreRol)
            .NotEmpty().WithMessage("ElNombre Rol  es obligatorio.")
            .MaximumLength(20).WithMessage("El Nombre Rol no puede tener más de 20 caracteres.");
    }
}
