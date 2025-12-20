using BiblotecaWeb.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class Permiso
{
    [Key]
    public int PermisoId { get; set; }
    public string NombrePermiso { get; set; }
    public bool Estado {  get; set; }
    public DateTime FechaRegistro { get; set; }= DateTime.Now;
    public ICollection<PermRol> PermRoles { get; set; } = new List<PermRol>();
}
