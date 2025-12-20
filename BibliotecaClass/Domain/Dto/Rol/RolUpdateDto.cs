using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Rol
{
    public class RolUpdateDto
    {
        public string NombreRol { get; set; }
        public bool Estado { get; set; }

    }
}
