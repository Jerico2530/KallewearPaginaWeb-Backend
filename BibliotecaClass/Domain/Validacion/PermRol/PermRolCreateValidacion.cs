using BiblotecaWeb.Domain.Dto.PermRol;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class PermRolCreateValidacion : AbstractValidator<PermRolCreateDto>
{
    public PermRolCreateValidacion()
    {
        RuleFor(x => x.PermisoId)
           .GreaterThan(0).WithMessage("Debe seleccionar un permiso válido.");

        RuleFor(x => x.RolId)
           .GreaterThan(0).WithMessage("Debe seleccionar un rol válido.");

    }
}
