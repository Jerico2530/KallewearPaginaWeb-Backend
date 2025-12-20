using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class ProductoCategoria
    {
        [Key]
        public int ProductoCategoriaId {  get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;




    }
}
