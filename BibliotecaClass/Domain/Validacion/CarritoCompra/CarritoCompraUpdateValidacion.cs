using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using FluentValidation;

namespace BiblotecaWeb.Model.Validacion.CarritoCompra;

public class CarritoCompraUpdateValidacion : AbstractValidator<CarritoCompraUpdateDto>
{
    public CarritoCompraUpdateValidacion()
    {
        RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");

        RuleFor(x => x.ProductoTallaId)
            .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");

        RuleFor(x => x.TotalCarrito)
            .GreaterThanOrEqualTo(0).WithMessage("El total del carrito no puede ser negativo.");
    }
}
