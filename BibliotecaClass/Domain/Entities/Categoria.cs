using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }
        public string DesCategoria {  get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;
        public ICollection<ProductoCategoria> ProductoCategorias { get; set; } = new List<ProductoCategoria>();
    }
}
