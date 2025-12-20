using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Moneda;

public class MonedaCreateDto
{

    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Simbolo { get; set; }
    public bool Estado { get; set; }

}
