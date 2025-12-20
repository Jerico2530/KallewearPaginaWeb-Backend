using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class Genero
{
    [Key]
    public int GeneroId { get; set; }
    public string Tipo { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
