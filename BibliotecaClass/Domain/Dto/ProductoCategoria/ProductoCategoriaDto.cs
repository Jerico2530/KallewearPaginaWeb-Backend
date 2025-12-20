using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.ProductoCategoria
{
    public class ProductoCategoriaDto
    {
        [Display(Name = "ID Producto Categoria")]
        public int ProductoCategoriaId { get; set; }
        [Display(Name = "ID Producto")]
        public int ProductoId { get; set; }
        [Display(Name = "Nombre Categoria")]
        public string Nombre { get; set; }
        [Display(Name = "Descripcion Categoria")]
        public string Descripcion { get; set; }
        [Display(Name = "Precio Categoria")]
        public string Precio { get; set; }
        [Display(Name = " Imagen Categoria")]
        public string Imagen { get; set; }
        [Display(Name = "ID Categoria")]
        public int CategoriaId { get; set; }
        [Display(Name = "ID Descripcion Categoria")]
        public string DesCategoria { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
