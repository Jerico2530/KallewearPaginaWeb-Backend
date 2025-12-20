using BiblotecaWeb.Domain.Dto.Genero;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class GeneroCreateValidacion : AbstractValidator<GeneroCreateDto>
{
    public GeneroCreateValidacion()
    {
        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("El Tipo genero   es obligatorio.")
            .MaximumLength(20).WithMessage("El Tipo genero  no puede tener más de 20 caracteres.");
    }
}
