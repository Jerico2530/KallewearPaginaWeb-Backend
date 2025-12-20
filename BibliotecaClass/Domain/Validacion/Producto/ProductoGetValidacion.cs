using FluentValidation;

namespace BiblotecaWeb;

public class ProductoGetValidacion : AbstractValidator<int>
{
    public ProductoGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto debe ser válido.");
    }
}
