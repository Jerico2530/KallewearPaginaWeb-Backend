using BiblotecaClass.Domain.Dto.Pago;

namespace BiblotecaWeb;

public class PagoUpdateDto
{

    public int OrdenId { get; set; }
    public int MedioPagoId { get; set; }
    public EstadoPago EstadoPago { get; set; }
    public bool Estado { get; set; }

}
