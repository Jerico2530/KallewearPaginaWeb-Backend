using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblotecaWeb;

public class DescuentoDto
{

    public int DescuentoId { get; set; }
    public string NombreDescuento { get; set; }
    public string Descripcion { get; set; }
    [Column(TypeName = "decimal(15,2)")]
    public decimal Porcentaje { get; set; }
    public string Imagen { get; set; }
    [DataType(DataType.Date)]
    public DateTime FechaInicio { get; set; }
    [DataType(DataType.Date)]
    public DateTime FechaFin { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
