using FluentValidation;

namespace BiblotecaWeb;

public class ProductoCategoriaGetValidacion : AbstractValidator<int>
{
    public ProductoCategoriaGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto-categoria debe ser válido.");
    }
}
