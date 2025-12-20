using BiblotecaWeb.Domain.Dto.Talla;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Talla
{
    public class TallaUpdateValidacion : AbstractValidator<TallaUpdateDto>
    {
        public TallaUpdateValidacion()
        {
            RuleFor(x => x.TipoTalla)
                .NotEmpty().WithMessage("El Tipo Talla  es obligatorio.")
                .MaximumLength(10).WithMessage("El Tipo Tallaa no puede tener más de 20 caracteres.");

            RuleFor(x => x.Descripcion)
                .NotEmpty().WithMessage("El Descripcion  es obligatorio.");
        }
    }
}

