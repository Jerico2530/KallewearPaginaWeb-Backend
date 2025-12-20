using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Talla
{
    public class TalllaDeleteValidacion : AbstractValidator<int>
    {
        public TalllaDeleteValidacion()
        {
            RuleFor(x => x)
                .GreaterThan(0).WithMessage("El ID del talla debe ser válido para eliminar.");
        }
    }
}
