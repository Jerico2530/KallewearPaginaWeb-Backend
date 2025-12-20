using BiblotecaWeb.Domain.Dto.Rol;
using FluentValidation;

namespace BiblotecaWeb;

public class RolUpdateValidacion : AbstractValidator<RolUpdateDto>
{
    public RolUpdateValidacion()
    {
        RuleFor(x => x.NombreRol)
            .NotEmpty().WithMessage("ElNombre Rol  es obligatorio.")
            .MaximumLength(20).WithMessage("El Nombre Rol no puede tener más de 20 caracteres.");

        RuleFor(x => x.Estado)
                .NotNull().WithMessage("El estado es obligatorio.")
                .Must(v => v == true || v == false)
                .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
    }
}
