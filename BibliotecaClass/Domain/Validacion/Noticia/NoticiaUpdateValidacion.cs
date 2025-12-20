using BiblotecaWeb.Domain.Dto.Noticia;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class NoticiaUpdateValidacion : AbstractValidator<NoticiaUpdateDto>
{
    public NoticiaUpdateValidacion()
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
