using BiblotecaWeb.Domain.Dto.Descuento;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Validacion.Descuento
{
    public class DescuentoCreateValidacion : AbstractValidator<DescuentoCreateDto>
    {
        public DescuentoCreateValidacion()
        {
            {
                RuleFor(x => x.NombreDescuento)
                    .NotEmpty().WithMessage("El nombre del descuento es obligatorio")
                    .MaximumLength(255).WithMessage("El nombre del descuento no puede superar 255 caracteres");


                RuleFor(x => x.Descripcion)
                    .NotEmpty().WithMessage("La descripción es obligatoria")
                    .MaximumLength(1000).WithMessage("La descripción no puede superar 1000 caracteres");


                RuleFor(x => x.Porcentaje)
                    .NotNull().WithMessage("El porcentaje es obligatorio")
                    .InclusiveBetween(0, 100).WithMessage("El porcentaje debe estar entre 0 y 100");


                RuleFor(x => x.Imagen)
                    .MaximumLength(200).WithMessage("La URL de la imagen no puede superar 200 caracteres")
                    .When(x => !string.IsNullOrEmpty(x.Imagen));


                RuleFor(x => x.FechaInicio)
                    .NotEmpty().WithMessage("La fecha de inicio es obligatoria");


                RuleFor(x => x.FechaFin)
                    .NotEmpty().WithMessage("La fecha de fin es obligatoria")
                    .Must((dto, fechaFin) => fechaFin >= dto.FechaInicio)
                    .WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio");


            }
        }
    }
}

