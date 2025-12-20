using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Noticia;

public class NoticiaCreateDto
{


    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }
    [DataType(DataType.Date)]
    public DateTime FechaPublicacion { get; set; }
    public bool Estado { get; set; }

}
