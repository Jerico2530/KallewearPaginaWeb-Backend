using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class OrdenCupon
{
    [Key]
    public int OrdenCuponId { get; set; }
    public int OrdenId { get; set; }
    public Orden Orden { get; set; }
    public int CuponId { get; set; }
    public Cupon Cupon { get; set; }
    public decimal MontoDescuento { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
