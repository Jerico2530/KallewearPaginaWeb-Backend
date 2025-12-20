using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Producto
{
    public class ProductoCreateDto
    {

        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int MonedaId { get; set; }
        public int GeneroId { get; set; }
        public string Imagen { get; set; }
        public bool Estado { get; set; }

    }
}
