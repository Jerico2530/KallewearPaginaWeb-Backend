using BiblotecaClass.Domain.Dto.Pago;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblotecaWeb.Domain.Entities;

public class Pago
{
    [Key]
    public int PagoId { get; set; }
    public int? InfoTarjetaId { get; set; } 
    public InfoTarjetas InfomaTarjetas { get; set; }
    public int OrdenId { get; set; }
    public Orden Orden { get; set; }
    public int? MedioPagoId { get; set; }
    public MedioPago MedioPago { get; set; }
    public string CodigoOperacion { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public EstadoPago EstadoPago { get; set; }




}
