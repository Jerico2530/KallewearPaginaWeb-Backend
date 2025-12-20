using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion {  get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Precio { get; set; }
        public string Imagen {  get; set; }
        public int MonedaId { get; set; }
        public Moneda Moneda { get; set; }
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public ICollection<ProductoCategoria> ProductoCategorias { get; set; } = new List<ProductoCategoria>();
        public ICollection<ProductoFavorito> ProductoFavoritos { get; set; } = new List<ProductoFavorito>();
        public ICollection<ProductoTalla> ProductoTallas { get; set; } = new List<ProductoTalla>();
        public ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();


    }
}
