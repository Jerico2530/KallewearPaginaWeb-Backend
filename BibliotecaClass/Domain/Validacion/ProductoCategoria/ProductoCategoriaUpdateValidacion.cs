using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class ProductoCategoriaUpdateValidacion : AbstractValidator<ProductoCategoriaUpdateDto>
{
    public ProductoCategoriaUpdateValidacion()
    {
        RuleFor(x => x.Estado)
                .NotNull().WithMessage("El estado es obligatorio.")
                .Must(v => v == true || v == false)
                .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
    }
}