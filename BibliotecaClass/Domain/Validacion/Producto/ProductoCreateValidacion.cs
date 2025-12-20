using BiblotecaWeb.Domain.Dto.Producto;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Producto
{
    public class ProductoCreateValidacion : AbstractValidator<ProductoCreateDto>
    {
        public ProductoCreateValidacion()
        {
            RuleFor(x => x.MonedaId)
           .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.GeneroId)
                .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("El Nombre  es obligatorio.")
                .MaximumLength(100).WithMessage("El Nombre no puede tener más de 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("El Descripcion  es obligatorio.");

            RuleFor(x => x.Precio)
                .NotNull().WithMessage("El precio unitario es obligatorio.");

            RuleFor(x => x.Imagen)
                .NotEmpty().WithMessage("El Imagen  es obligatorio.")
                .MaximumLength(500).WithMessage("El Imagen no puede tener más de 500 url.");

        }
    }
}
