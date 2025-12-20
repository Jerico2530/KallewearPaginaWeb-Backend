using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class OrdenDetalle
    {
        [Key]
        public int OrdenDetalleId { get; set; }
        public int OrdenId { get; set; }
        public Orden Orden { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

    }

}
