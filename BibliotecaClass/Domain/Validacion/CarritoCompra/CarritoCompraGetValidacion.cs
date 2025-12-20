using FluentValidation;

namespace BiblotecaWeb;

public class CarritoCompraGetValidacion : AbstractValidator<int>
{
    public CarritoCompraGetValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del carrito debe ser válido.");
    }
}
