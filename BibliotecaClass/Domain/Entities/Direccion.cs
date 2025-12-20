using BiblotecaWeb.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class Direccion
{
    [Key]
    public int DireccionId { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public string Departamento { get; set; }
    public string Provincia { get; set; }
    public string Distrito { get; set; }
    public string Via { get; set; }
    public string Numero { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
}
