using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Talla;

public class TallaUpdateDto
{

    public string TipoTalla { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }

}
