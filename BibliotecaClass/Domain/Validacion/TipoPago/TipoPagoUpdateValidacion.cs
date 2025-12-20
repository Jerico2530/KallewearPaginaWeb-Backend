using BiblotecaWeb.Domain.Dto.TipoPago;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class TipoPagoUpdateValidacion : AbstractValidator<TipoPagoUpdateDto>
{
    public TipoPagoUpdateValidacion()
    {
        RuleFor(x => x.DescripcionTipoPago)
            .NotEmpty().WithMessage("El Descripcion TipoPago  es obligatorio.")
            .MaximumLength(50).WithMessage("El Descripcion TipoPago no puede tener más de 50 caracteres.");
    }
}
