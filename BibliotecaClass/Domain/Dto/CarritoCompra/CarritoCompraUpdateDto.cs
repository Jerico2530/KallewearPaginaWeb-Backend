using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Validacion.CarritoCompra;

public class CarritoCompraUpdateDto
{
    public int UsuarioId { get; set; }
    public int ProductoTallaId { get; set; }
    public int? OrdenId { get; set; }
    public int Cantidad { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotal { get; private set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalCarrito { get; set; }
    public bool? Estado { get; set; }

}
