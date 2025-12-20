using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class Testimonio
    {
        [Key]
        public int TestimonioId { get; set; }
        public string Descripcion { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        [Range(0, 5, ErrorMessage = "La evaluación debe estar entre 0 y 5.")]
        public int Evaluacion { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;


    }
}
