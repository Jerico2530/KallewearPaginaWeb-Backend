using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        public string NombreRol {  get; set; }
        public bool  Estado {  get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public ICollection<UserRol> UserRoles { get; set; } = new List<UserRol>();
        public ICollection<PermRol> PermRoles { get; set; } = new List<PermRol>();
    }
}
