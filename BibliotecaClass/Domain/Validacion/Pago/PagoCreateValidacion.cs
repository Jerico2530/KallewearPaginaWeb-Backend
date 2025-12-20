using BiblotecaWeb.Domain.Dto.Pago;
using BiblotecaWeb.Model.Dto;
using FluentValidation;

namespace BiblotecaWeb;

public class PagoCreateValidacion : AbstractValidator<PagoCreateDto>
{
    public PagoCreateValidacion()
    {
        RuleFor(x => x.OrdenId)
           .GreaterThan(0).WithMessage("Debe seleccionar un orden válido.");

        RuleFor(x => x.MedioPagoId)
            .GreaterThan(0).WithMessage("Debe seleccionar un medio pago válido.");

        RuleFor(x => x.CodigoOperacion)
            .MaximumLength(100).WithMessage("El Codigo Operacion no puede tener más de 100 caracteres.");
    }
}
