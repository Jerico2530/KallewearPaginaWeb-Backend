using BiblotecaClass.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class DetalleTarjeta
{
    [Key]
    public int DetalleTarjetaId { get; set; }
    public string NumeroTarjeta { get; set; }
    public string FechaVencimiento { get; set; }
    public string CVV {  get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; }= DateTime.Now;
    public ICollection<InfoTarjetas> InfomaTarjetas { get; set; } = new List<InfoTarjetas>();

}
