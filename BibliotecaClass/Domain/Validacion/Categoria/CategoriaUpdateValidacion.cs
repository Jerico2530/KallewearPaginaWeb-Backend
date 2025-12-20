using BiblotecaWeb.Domain.Dto.Categoria;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Categoria
{
    public class CategoriaUpdateValidacion : AbstractValidator<CategoriaUpdateDto>
    {
        public CategoriaUpdateValidacion()
        {
            {
                RuleFor(x => x.DesCategoria)
                    .NotEmpty().WithMessage("La descripcion  es obligatorio.")
                    .MaximumLength(150).WithMessage("La descripcion no puede tener más de 150 caracteres.");

            }
        }
    }
}
