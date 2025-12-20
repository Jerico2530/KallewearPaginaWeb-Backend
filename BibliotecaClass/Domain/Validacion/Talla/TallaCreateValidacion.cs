using BiblotecaWeb.Domain.Dto.Talla;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class TallaCreateValidacion : AbstractValidator<TallaCreateDto>
{
    public TallaCreateValidacion()
    {
        RuleFor(x => x.TipoTalla)
            .NotEmpty().WithMessage("El Tipo Talla  es obligatorio.")
            .MaximumLength(10).WithMessage("El Tipo Tallaa no puede tener más de 20 caracteres.");

        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("El Descripcion  es obligatorio.");
    }
}
