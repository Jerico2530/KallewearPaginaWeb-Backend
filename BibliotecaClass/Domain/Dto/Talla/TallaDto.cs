using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Talla
{
    public class TallaDto
    {
        [Display(Name = "ID Talla")]
        public int TallaId { get; set; }
        [Display(Name = "Tipo de Talla")]
        public string TipoTalla { get; set; }
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        [Display(Name = "Estado")]
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
