using BiblotecaWeb.Domain.Dto.Categoria;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class CategoriaCreateValidacion : AbstractValidator<CategoriaCreateDto>
{
    public CategoriaCreateValidacion()
    {
        {
            RuleFor(x => x.DesCategoria)
                .NotEmpty().WithMessage("La descripcion  es obligatorio.")
                .MaximumLength(150).WithMessage("La descripcion no puede tener más de 150 caracteres.");

        }
    }
}
