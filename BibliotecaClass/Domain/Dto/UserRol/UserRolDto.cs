using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.UserRol
{
    public class UserRolDto
    {
        [Display(Name = "ID Usuario-Rol")]
        public int UserRolId { get; set; }
        [Display(Name = "ID Usuario")]
        public int UsuarioId { get; set; }
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }
        [Display(Name = "Apellido Completo")]
        public string? ApellidoCompleto { get; set; }
        [Display(Name = "DNI")]
        public string? DNI { get; set; }
        [Display(Name = "Correo Electrónico")]
        public string? CorreoElectronico { get; set; }
        [Display(Name = "ID Rol")]
        public int RolId { get; set; }
        [Display(Name = "Rol")]
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
