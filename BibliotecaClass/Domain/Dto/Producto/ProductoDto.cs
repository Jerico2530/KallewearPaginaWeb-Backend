using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Producto
{
    public class ProductoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; }
        public string Imagen { get; set; }
        public int MonedaId { get; set; }
        public string Codigo { get; set; }
        public int GeneroId { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
