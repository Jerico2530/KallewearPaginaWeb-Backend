using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Validacion.CarritoCompra;

public class CarritoCompraCreateDto
{
    public int UsuarioId { get; set; }
    public int ProductoTallaId { get; set; }
    public int? OrdenId { get; set; }
    public int Cantidad { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }
    public bool Estado { get; set; }
}
