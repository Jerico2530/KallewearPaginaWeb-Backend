using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class TipoPago
{
    [Key]
    public int TipoPagoId { get; set; }
    public string DescripcionTipoPago { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<MedioPago> MediosPagos { get; set; } = new List<MedioPago>();
}
