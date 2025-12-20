using BiblotecaWeb.Domain.Entities;

namespace BiblotecaWeb.Domain.Dto.OrdenDetalle;
using BiblotecaWeb.Domain.Entities;

public class OrdenDetalleUpdateDto
{
    public int OrdenId { get; set; }
    public Orden Orden { get; set; }
    public int ProductoId { get; set; }
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal { get; set; }
    public bool Estado { get; set; }

}
