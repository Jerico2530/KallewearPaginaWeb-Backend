using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblotecaWeb.Domain.Entities;

public class CarritoCompra
{
    [Key]
    public int CarritoId {  get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public int ProductoTallaId {  get; set; }
    public ProductoTalla ProductoTalla { get; set; }
    public int? OrdenId { get; set; }
    public Orden Orden { get; set; }
    public int Cantidad { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotal { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalCarrito { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;


}
