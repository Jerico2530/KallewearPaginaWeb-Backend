namespace BiblotecaWeb.Domain.Dto.DetalleTarjeta;

public class DetalleTarjetaUpdateDto
{
    public string NumeroTarjeta { get; set; }
    public string FechaVencimiento { get; set; }
    public string CVV { get; set; }
    public bool Estado { get; set; }

}
