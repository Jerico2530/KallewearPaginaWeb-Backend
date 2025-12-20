using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class OrdenDetalleCreateValidacion : AbstractValidator<OrdenDetalleCreateDto>
{
    public OrdenDetalleCreateValidacion()
    {
        RuleFor(x => x.OrdenId)
            .GreaterThan(0).WithMessage("Debe seleccionar un orden válido.");

        RuleFor(x => x.ProductoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("Debe seleccionar un cantidad válido.");

    }
}
