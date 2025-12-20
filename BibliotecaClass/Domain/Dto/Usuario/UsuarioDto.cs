using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BiblotecaWeb.Domain.Dto.Usuario
{
    public class UsuarioDto
    {
        [Key]

        [Display(Name = "ID Usuario")]
        public int UsuarioId { get; set; }
        [Display(Name = "Nombre Completo")]
        public string? NombreCompleto { get; set; }
        [Display(Name = "Apellido Completo")]
        public string? ApellidoCompleto { get; set; }
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        [Display(Name = "DNI")]
        public string? DNI { get; set; }
        public string Imagen { get; set; }
        [Display(Name = "Correo Electrónico")]
        public string? CorreoElectronico { get; set; }
        public string? Contraseña { get; set; }
        public string? ContraseñaVisible { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
