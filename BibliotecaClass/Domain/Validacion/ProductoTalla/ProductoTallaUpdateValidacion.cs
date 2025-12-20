using BiblotecaWeb.Domain.Dto.ProductoTalla;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class ProductoTallaUpdateValidacion : AbstractValidator<ProductoTallaUpdateDto>
{
    public ProductoTallaUpdateValidacion()
    {
    
        RuleFor(x => x.Stock)
           .NotNull().WithMessage("El Stock es obligatorio.")
           .GreaterThanOrEqualTo(0).WithMessage("El Stock debe ser mayor o igual a cero.");
        RuleFor(x => x.Estado)
                .NotNull().WithMessage("El estado es obligatorio.")
                .Must(v => v == true || v == false)
                .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
    }
}
