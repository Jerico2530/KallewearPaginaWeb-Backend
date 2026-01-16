using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaClass.Domain.Dto.InfoTarjetas
{
    public class InfoTarjetaDto
    {
        [Display(Name = "ID Info Tarjeta")]
        public int InfoTarjetaId { get; set; }
        [Display(Name = "ID Usuario")]
        public int UsuarioId { get; set; }
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Display(Name = "ID Detalle Tarjeta")]
        public int DetalleTarjetaId { get; set; }
        [Display(Name = "Numero Tarjeta")]
        public string NumeroTarjeta { get; set; }
        [Display(Name = "Fecha Vencimiento")]
        public string FechaVencimiento { get; set; }
        [Display(Name = "CVV")]
        public string CVV { get; set; }
        [Display(Name = "ID Medio Pago")]
        public int MedioPagoId { get; set; }
        [Display(Name = "Descripcion Medio Pago")]
        public string DescripcionMedioPago { get; set; }
        [Display(Name = "Descripcion Tipo Pago")]
        public string TipoPago { get; set; }
        [Display(Name = "Estadp")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
