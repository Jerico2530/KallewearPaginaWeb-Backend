using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Model.Dto
{
    public class UserRolCreateDto
    {

        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public bool Estado { get; set; }

    }
}
