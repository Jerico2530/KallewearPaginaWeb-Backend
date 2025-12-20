using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BiblotecaWeb.Model.Dto;

namespace BiblotecaWeb.Domain.Validacion.CarritoCompra;

public class CarritoCompraDto
{
    public int CarritoId { get; set; }
    public int ProductoTallaId { get; set; }
    public int UsuarioId { get; set; }
    public string? NombreCompleto { get; set; }
    public string? ApellidoCompleto { get; set; }
    public string? DNI { get; set; }
    public string? CorreoElectronico { get; set; }
    public int ProductoId { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Precio { get; set; }
    public string Imagen { get; set; }
    public string Moneda { get; set; }
    public string Genero { get; set; }
    public int TallaId { get; set; }
    public string TipoTalla { get; set; }
    public OrdenSimpleDto Orden { get; set; }
    public int Cantidad { get; set; }


    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioUnitario { get; set; }
    [Column(TypeName = "decimal(10,2)")]
    public decimal SubTotal { get; private set; }
    public decimal TotalCarrito { get; set; }

    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
