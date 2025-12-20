using FluentValidation;

namespace BiblotecaWeb;

public class ProductoDeleteValidacion : AbstractValidator<int>
{
    public ProductoDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto debe ser válido para eliminar.");
    }
}