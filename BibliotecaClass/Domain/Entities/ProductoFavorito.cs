using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class ProductoFavorito
    {
        [Key]
        public int ProductoFavoritoId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public bool Estado {  get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;
    }
}
