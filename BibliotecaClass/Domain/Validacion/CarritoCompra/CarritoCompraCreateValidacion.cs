using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.CarritoCompra
{
    public class CarritoCompraCreateValidacion : AbstractValidator<CarritoCompraCreateDto>
    {
        public CarritoCompraCreateValidacion()
        {
            RuleFor(x => x.UsuarioId)
            .GreaterThan(0).WithMessage("Debe seleccionar un usuario válido.");

            RuleFor(x => x.ProductoTallaId)
                .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

            RuleFor(x => x.PrecioUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("El precio unitario no puede ser negativo.");

        }
    }
}
