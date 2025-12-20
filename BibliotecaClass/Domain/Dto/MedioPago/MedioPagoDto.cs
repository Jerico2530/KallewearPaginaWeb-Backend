namespace BiblotecaWeb.Domain.Dto.MedioPago;
using System.ComponentModel.DataAnnotations;

public class MedioPagoDto
{
    [Display(Name = "ID Medio Pago")]
    public int MedioPagoId { get; set; }
    [Display(Name = "ID Tipo Pago")]
    public int TipoPagoId { get; set; }
    [Display(Name = "Tipo Pago")]
    public string DescripcionTipoPago { get; set; }
    [Display(Name = "Medio Pago")]
    public string DescripcionMedioPago { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
