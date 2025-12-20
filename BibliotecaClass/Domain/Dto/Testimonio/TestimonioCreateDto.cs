using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.Testimonio
{
    public class TestimonioCreateDto
    {
        public string Descripcion { get; set; }
        public int UsuarioId { get; set; }
        public int Evaluacion { get; set; }
        public bool Estado { get; set; }

    }
}
