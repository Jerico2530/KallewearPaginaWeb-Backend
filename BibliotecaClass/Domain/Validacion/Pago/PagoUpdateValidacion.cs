using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class PagoUpdateValidacion : AbstractValidator<PagoUpdateDto>
{
    public PagoUpdateValidacion()
    {
        RuleFor(x => x.OrdenId)
           .GreaterThan(0).WithMessage("Debe seleccionar un orden válido.");

    }
}
