namespace BiblotecaWeb.Domain.Dto.Pago;

public class PagoCreateDto
{

    public int OrdenId { get; set; }
    public int MedioPagoId { get; set; }
    public string CodigoOperacion { get; set; }
    public bool Estado { get; set; }

}
