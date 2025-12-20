                    using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class ProductoTalla
{
    [Key]
    public int ProductoTallaId { get; set; }
    public int ProductoId { get; set; }
    public Producto  Producto { get; set; }
    public int TallaId { get; set; }
    public Talla Talla { get; set; }
    public int Stock {  get; set; }
    public int StockReservado { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<CarritoCompra> CarritoCompras { get; set; } = new List<CarritoCompra>();
}
