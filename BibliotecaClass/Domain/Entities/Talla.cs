using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class Talla
{
    [Key]
    public int TallaId { get; set; }
    public string TipoTalla { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<ProductoTalla> ProductoTallas { get; set; } = new List<ProductoTalla>();
}
