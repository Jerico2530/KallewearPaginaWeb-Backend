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

    }
}
