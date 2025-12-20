using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.TipoPago
{
    public class TipoPagoDto
    {
        [Display(Name = "ID Tipo Pago")]
        public int TipoPagoId { get; set; }
        [Display(Name = "Descripción Tipo Pago")]
        public string DescripcionTipoPago { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

    }
}
