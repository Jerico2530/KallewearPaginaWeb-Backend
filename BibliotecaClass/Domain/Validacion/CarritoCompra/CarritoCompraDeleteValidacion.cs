using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.CarritoCompra;

public class CarritoCompraDeleteValidacion : AbstractValidator<int>
{
    public CarritoCompraDeleteValidacion()
    {
        RuleFor(x => x)
            .GreaterThan(0).WithMessage("El ID del carrito debe ser válido para eliminar.");
    }
}
