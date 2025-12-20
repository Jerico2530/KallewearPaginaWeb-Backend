using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.ProductoTalla
{
    public class ProductoTallaDto
    {
        [Display(Name = "ID Producto Talla")]
        public int ProductoTallaId { get; set; }
        [Display(Name = "ID Producto")]
        public int ProductoId { get; set; }
        [Display(Name = "Nombre Producto")]
        public string Nombre { get; set; }
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
        [Display(Name = "Moneda")]
        public string Moneda { get; set; }
        [Display(Name = "Genero")]
        public string Genero { get; set; }
        [Display(Name = "Imagen")]
        public string Imagen { get; set; }
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }
        [Display(Name = "ID Talla")]
        public int TallaId { get; set; }
        [Display(Name = "Tipo Talla")]
        public string TipoTalla { get; set; }
        [Display(Name = "Valor Talla")]
        public int Stock { get; set; }
        [Display(Name = "Stock Reservado")]
        public int StockReservado { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
