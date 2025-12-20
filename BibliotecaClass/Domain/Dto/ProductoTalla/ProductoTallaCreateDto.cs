using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class ProductoTallaCreateDto
{
    public int ProductoId { get; set; }
    public int TallaId { get; set; }
    public int Stock { get; set; }
    public bool Estado { get; set; }

}
