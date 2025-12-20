using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Dto.ProductoTalla
{
    public class ProductoTallaUpdateDto
    {

        public int Stock { get; set; }
        public int StockReservado { get; set; }
        public bool Estado { get; set; }


    }
}
