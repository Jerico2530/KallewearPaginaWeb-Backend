using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class UserRol
    {
        
        public int UserRolId { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public bool Estado {  get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;


    }
}
