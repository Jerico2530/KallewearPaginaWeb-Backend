using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class MedioPago
{
    [Key]
    public int MedioPagoId { get; set; }
    public int TipoPagoId { get; set; }
    public TipoPago TipoPago { get; set; }
    public string DescripcionMedioPago { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
