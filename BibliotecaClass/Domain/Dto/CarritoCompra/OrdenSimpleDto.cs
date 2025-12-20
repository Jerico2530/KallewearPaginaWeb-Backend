using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Model.Dto
{
    public class OrdenSimpleDto
    {
        public int OrdenId { get; set; }
        public int UsuarioId { get; set; }
        public string MetodoEntrega { get; set; }
        public decimal Total { get; set; }
        public int DireccionId { get; set; }
    }
}
