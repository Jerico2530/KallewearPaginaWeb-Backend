using BiblotecaWeb.Domain.Dto.Orden;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class OrdenCreateValidacion : AbstractValidator<OrdenCreateDto>
{
    public OrdenCreateValidacion()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");

        RuleFor(x => x.SucursalId)
            .GreaterThan(0).WithMessage("Debe seleccionar un sucursal válido.");

        RuleFor(x => x.MetodoEntrega)
            .NotEmpty().WithMessage("El Metodo Entrega  es obligatorio.")
            .MaximumLength(20).WithMessage("El Metodo Entrega no puede tener más de 20 caracteres.");

        RuleFor(x => x.DireccionId)
            .GreaterThan(0).WithMessage("Debe seleccionar un Direccion válido.");

    }
}
