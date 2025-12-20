using BiblotecaWeb.Domain.Dto.OrdenDetalle;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.OrdenDetalle
{
    public class OrdenDetalleUpdateValidacion : AbstractValidator<OrdenDetalleUpdateDto>
    {
        public OrdenDetalleUpdateValidacion()
        {
            RuleFor(x => x.OrdenId)
                .GreaterThan(0).WithMessage("Debe seleccionar un orden válido.");

            RuleFor(x => x.ProductoId)
                .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("Debe seleccionar un cantidad válido.");

        }
    }
}
