using BiblotecaClass.Domain.Entities;
using BiblotecaWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaClass.Domain.Entities
{
    public class InfoTarjetas
    {
        [Key]
        public int InfoTarjetaId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int DetalleTarjetaId { get; set; }
        public DetalleTarjeta DetalleTarjeta { get; set; }
        public int MedioPagoId { get; set; }
        public MedioPago MedioPago { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();


    }
}
