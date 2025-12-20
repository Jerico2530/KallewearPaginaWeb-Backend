using BiblotecaWeb.Model;

namespace BiblotecaWeb.Domain.Dto.PermRol;

public class PermRolCreateDto
{
    public int PermisoId { get; set; }
    public int RolId { get; set; }
    public bool Estado { get; set; }
}
