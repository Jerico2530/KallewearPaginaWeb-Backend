using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Usuario
{
    public class AuthResponseDto
    {
        public int UsuarioId { get; set; }
        public string? NombreCompleto { get; set; }
        public string? CorreoElectronico { get; set; }

        // 🔑 Para autenticación
        public List<string> Roles { get; set; } = new();
        public List<string> Permisos { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}
