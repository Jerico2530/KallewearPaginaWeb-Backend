using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Cupon;

public class CuponDto
{
    public int CuponId { get; set; }
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public decimal Descuento { get; set; }
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

}
