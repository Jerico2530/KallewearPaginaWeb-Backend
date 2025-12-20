using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb.Domain.Dto.Usuario;

public class UsuarioLoginDto
{

    public string CorreoElectronico { get; set; }
    public string Contraseña { get; set; }

}
