using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Orden;

public class OrdenDto
{
    [Display(Name = "ID Orden")]
    public int OrdenId { get; set; }
    [Display(Name = "ID Usuario")]
    public int UsuarioId { get; set; }
    [Display(Name = "Nombre Completo")]
    public string? NombreCompleto { get; set; }
    [Display(Name = "Apellido Completo")]
    public string? ApellidoCompleto { get; set; }
    [Display(Name = "DNI")]
    public string? DNI { get; set; }
    [Display(Name = "ID Sucursal")]
    public int? SucursalId { get; set; }
    public string? Locales { get; set; }
    public string? Descripcion { get; set; }
    [Display(Name = "Metodo Entrega")]
    public string MetodoEntrega { get; set; }
    [Display(Name = "ID Direccion")]
    public int? DireccionId { get; set; }
    public string Departamente { get; set; }
    public string Provincia { get; set; }
    public string Distrito { get; set; }
    public string Via { get; set; }
    public string Numero { get; set; }
    public bool Estado { get; set; }
    public decimal? Total { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<CarritoCompraDto> CarritoCompras { get; set; } = new List<CarritoCompraDto>();
}
