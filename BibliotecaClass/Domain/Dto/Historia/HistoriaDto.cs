using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class HistoriaDto
{
    [Key]
    public int HistoriaId { get; set; }
    [DataType(DataType.Date)]
    public DateTime Año { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
