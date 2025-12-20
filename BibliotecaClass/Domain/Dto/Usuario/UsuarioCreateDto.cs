using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Usuario
{
    public class UsuarioCreateDto
    {
        public string? NombreCompleto { get; set; }
        public string? ApellidoCompleto { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }
        public string DNI { get; set; }
        public string? Imagen { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Contraseña { get; set; }
        public string? ContraseñaVisible { get; set; }
        public bool Estado { get; set; }
    }
}
