using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class ProductoCategoriaCreateValidacion : AbstractValidator<ProductoCategoriaCreateDto>
{
    public ProductoCategoriaCreateValidacion()
    {
        RuleFor(x => x.ProductoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Debe seleccionar un categoria válido.");
    }
}
