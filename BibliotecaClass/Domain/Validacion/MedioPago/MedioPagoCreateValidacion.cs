using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class MedioPagoCreateValidacion : AbstractValidator<MedioPagoCreateDto>
{
    public MedioPagoCreateValidacion()
    {
        RuleFor(x => x.TipoPagoId)
           .GreaterThan(0).WithMessage("Debe seleccionar un Tipo Pago válido.");

        RuleFor(x => x.DescripcionMedioPago)
            .NotEmpty().WithMessage("La Descripcion Medio Pago  es obligatorio.")
            .MaximumLength(100).WithMessage("La Descripcion Medio Pago no puede tener más de 100 caracteres.");
    }
}