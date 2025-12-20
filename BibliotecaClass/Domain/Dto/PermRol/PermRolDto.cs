using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class PermRolDto
{
    [Display(Name = "ID Permiso-Rol")]
    public int PermRolId { get; set; }
    [Display(Name = "ID Permiso")]
    public int PermisoId { get; set; }
    [Display(Name = "Permiso")]
    public string  NombrePermiso { get; set; }
    [Display(Name = "ID Rol")]
    public int RolId { get; set; }
    [Display(Name = "Rol")]
    public string  NombreRol { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
