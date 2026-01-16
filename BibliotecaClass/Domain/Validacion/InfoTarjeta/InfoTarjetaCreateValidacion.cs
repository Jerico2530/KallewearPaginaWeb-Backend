using BiblotecaClass.Domain.Dto.InfoTarjetas;
using BiblotecaWeb;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaClass.Domain.Validacion.InfoTarjeta
{
    public class InfoTarjetaCreateValidacion : AbstractValidator<InfoTarjetaCreateDto>
    {
        public InfoTarjetaCreateValidacion()
        {
            RuleFor(x => x.DetalleTarjetaId)
               .GreaterThan(0).WithMessage("Debe seleccionar un Detalle Tarjeta válido.");

            RuleFor(x => x.MedioPagoId)
                .GreaterThan(0).WithMessage("Debe seleccionar un Medio Pago válido.");
        }
    }
}
