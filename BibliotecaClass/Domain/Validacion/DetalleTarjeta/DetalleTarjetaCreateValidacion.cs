using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.DetalleTarjeta
{
    public class DetalleTarjetaCreateValidacion : AbstractValidator<DetalleTarjetaCreateDto>
    {
        public DetalleTarjetaCreateValidacion()
        {
            RuleFor(x => x.PagoId)
                .GreaterThan(0).WithMessage("Debe seleccionar un producto válido.");

            RuleFor(x => x.NumeroTarjeta)
                .NotEmpty().WithMessage("El Numero Tarjeta  es obligatorio.")
                .MaximumLength(20).WithMessage("El Numero Tarjeta no puede tener más de 20 caracteres.")
                .Matches(@"^\d{13,20}$").WithMessage("El número de tarjeta debe contener solo dígitos ");


            RuleFor(x => x.FechaVencimiento)
               .NotEmpty().WithMessage("La fecha de vencimiento es obligatoria.")
               .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$").WithMessage("El formato debe ser MM/YY.");

            RuleFor(x => x.CVV)
                .NotEmpty().WithMessage("El CVV   es obligatorio.")
                .MaximumLength(4).WithMessage("El CVV no puede tener más de 4 caracteres.")
                .Matches(@"^\d{3,4}$").WithMessage("El CVV debe tener  dígitos numéricos.");
        }
    }
}
