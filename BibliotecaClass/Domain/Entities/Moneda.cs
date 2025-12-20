using BiblotecaWeb.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class Moneda
{
    [Key]
    public int MonedaId { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string Simbolo { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
