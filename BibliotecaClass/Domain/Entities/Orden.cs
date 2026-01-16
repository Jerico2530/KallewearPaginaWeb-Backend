using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class Orden
{
    [Key]
    public int OrdenId { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public int? SucursalId { get; set; }
    public Sucursal Sucursal { get; set; }
    public string MetodoEntrega { get; set; }
    public int? DireccionId { get; set; }
    public Direccion Direccion { get; set; }
    public bool? Estado { get; set; }
    public decimal? Total { get; set; }   
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    public ICollection<CarritoCompra> CarritoCompras { get; set; } = new List<CarritoCompra>();
    public ICollection<OrdenDetalle> OrdenDetalles { get; set; } = new List<OrdenDetalle>();
}

