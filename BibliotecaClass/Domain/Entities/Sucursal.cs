using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class Sucursal
{
    [Key]
    public int SucursalId { get; set; }
    public string Locales { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
}
