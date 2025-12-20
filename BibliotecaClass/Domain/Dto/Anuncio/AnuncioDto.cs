using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Model.Dto
{
    public  class AnuncioDto
    {
        [Display(Name = "ID Anucnio")]
        public int AnuncioId { get; set; }

        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [DataType(DataType.Date)]
        public DateTime FechaFinal { get; set; }
        public int Orden { get; set; }
        public bool Estado { get; set; }
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
