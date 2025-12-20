using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Entities;

public class PermRol
{
    [Key]
    public int PermRolId { get; set; }
    public int PermisoId { get; set; }
    public Permiso Permiso { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
