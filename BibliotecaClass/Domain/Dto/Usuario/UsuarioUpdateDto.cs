using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Model.Dto
{
    public class UsuarioUpdateDto
    {
        public string NombreCompleto { get; set; }
        public string ApellidoCompleto { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public string? DNI { get; set; }
        public string Imagen { get; set; }
        public string CorreoElectronico { get; set; }
        public bool Estado { get; set; }

    }
}
