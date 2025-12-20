using BiblotecaWeb.Domain.Dto.ProductoTalla;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.ProductoTalla
{
    public class ProductoTallaCreateValidacion : AbstractValidator<ProductoTallaCreateDto>
    {
        public ProductoTallaCreateValidacion()
        {
            RuleFor(x => x.ProductoId)
           .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");
            RuleFor(x => x.TallaId)
          .GreaterThan(0).WithMessage("Debe seleccionar una talla válido.");

            RuleFor(x => x.Stock)
               .NotNull().WithMessage("El Stock es obligatorio.")
               .GreaterThanOrEqualTo(0).WithMessage("El Stock debe ser mayor o igual a cero.");
            RuleFor(x => x.Estado)
                    .NotNull().WithMessage("El estado es obligatorio.")
                    .Must(v => v == true || v == false)
                    .WithMessage("El estado debe ser verdadero o falso (1 o 0).");
        }
    }
}
