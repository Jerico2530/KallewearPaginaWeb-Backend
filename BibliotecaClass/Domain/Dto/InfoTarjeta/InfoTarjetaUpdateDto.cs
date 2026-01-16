using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaClass.Domain.Dto.InfoTarjetas
{
    public class InfoTarjetaUpdateDto
    {
        public int DetalleTarjetaId { get; set; }
        public int MedioPagoId { get; set; }
        public bool Estado { get; set; }
    }
}
