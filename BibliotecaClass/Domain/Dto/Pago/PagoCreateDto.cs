using BiblotecaClass.Domain.Dto.Pago;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;

namespace BiblotecaWeb.Domain.Dto.Pago;

public class PagoCreateDto
{

    public int OrdenId { get; set; }
    public int? InfoTarjetaId { get; set; } 
    public int? MedioPagoId { get; set; }
    public string CodigoOperacion { get; set; }
    public bool Estado { get; set; }
    public DetalleTarjetaCreateDto? NuevaTarjeta { get; set; }

}
