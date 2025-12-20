using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Moneda;

public class MonedaDto
{
    [Key]
    [Display(Name = "ID Moneda")]
    public int MonedaId { get; set; }

    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Simbolo { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
