using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class DetalleTarjeta
{
    [Key]
    public int DetalleTarjetaId { get; set; }
    public int PagoId { get; set; }
    public Pago Pago { get; set; }
    public string NumeroTarjeta { get; set; }
    public string FechaVencimiento { get; set; }
    public string CVV {  get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; }= DateTime.Now;
}
