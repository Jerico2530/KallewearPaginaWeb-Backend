using FluentValidation;

namespace BiblotecaWeb;

public class ProductoTallaGetValidacion : AbstractValidator<int>
{
    public ProductoTallaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto-talla debe ser válido.");
    }
}
