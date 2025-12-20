using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Historia;

public class HistoriaUpdateDto
{
    [DataType(DataType.Date)]
    public DateTime Año { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }

}
