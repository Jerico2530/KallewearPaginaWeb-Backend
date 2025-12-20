using BiblotecaWeb.Domain.Dto.Noticia;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Noticia
{
    public class NoticiaCreateValidacion : AbstractValidator<NoticiaCreateDto>
    {
        public NoticiaCreateValidacion()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("El Titulo  es obligatorio.")
                .MaximumLength(100).WithMessage("El Titulo no puede tener más de 100 caracteres.");

            RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("El Numero Tarjeta  es Descripcion.");

            RuleFor(x => x.Imagen)
            .MaximumLength(200).WithMessage("El Imagen no puede tener más de 200 url.");

            RuleFor(x => x.FechaPublicacion)
               .NotEmpty().WithMessage("La Fecha Publicacion es obligatoria.");
        }
    }
}

