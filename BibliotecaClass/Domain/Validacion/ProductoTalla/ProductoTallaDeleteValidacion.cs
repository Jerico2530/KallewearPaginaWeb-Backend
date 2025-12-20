using FluentValidation;

namespace BiblotecaWeb;

public class ProductoTallaDeleteValidacion : AbstractValidator<int>
{
    public ProductoTallaDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del producto-talla debe ser válido para eliminar.");
    }
}
