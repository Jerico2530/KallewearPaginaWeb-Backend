using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Pago
{
    public class PagoDto
    {
        [Display(Name = "ID Pago")]
        public int PagoId { get; set; }
        public int InfoTarjetaId { get; set; }
        [Display(Name = "ID Orden")]
        public int OrdenId { get; set; }
        [Display(Name = "Medio Pago")]
        public string MetodoEntrega { get; set; }
        [Display(Name = "ID Medio Pago")]
        public int? MedioPagoId { get; set; }
        [Display(Name = "Descripcion Medio Pago")]
        public string DescripcionMedioPago { get; set; }
        [Display(Name = "Tipo de Pago")]
        public string TipoPago { get; set; }
        [Display(Name = "Código de Operación")]
        public string CodigoOperacion { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<DetalleTarjetaDto> DetalleTarjetas { get; set; }
    }
}
