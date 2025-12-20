using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Testimonio
{
    public class TestimonioDto
    {
        [Display(Name = "ID Testimonio")]
        public int TestimonioId { get; set; }
        public string Descripcion { get; set; }
        [Display(Name = "ID Usuario")]
        public int UsuarioId { get; set; }
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }
        public string Imagen { get; set; }

        public int Evaluacion { get; set; }
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
