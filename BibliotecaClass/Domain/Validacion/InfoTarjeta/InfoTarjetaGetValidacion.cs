using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaClass.Domain.Validacion.InfoTarjeta
{
    public class InfoTarjetaGetValidacion : AbstractValidator<int>
    {
        public InfoTarjetaGetValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del Info Tarjeta debe ser válido.");
        }
    }
}
