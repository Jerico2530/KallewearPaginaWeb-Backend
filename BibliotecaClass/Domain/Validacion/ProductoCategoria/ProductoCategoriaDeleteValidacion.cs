using FluentValidation;

namespace BiblotecaWeb;

public class ProductoCategoriaDeleteValidacion : AbstractValidator<int>
{
    public ProductoCategoriaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto-Categiria debe ser válido para eliminar.");
    }
}
