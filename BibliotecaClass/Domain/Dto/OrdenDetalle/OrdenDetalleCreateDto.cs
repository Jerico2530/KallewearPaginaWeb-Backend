using BiblotecaWeb.Domain.Entities;

namespace BiblotecaWeb;

public class OrdenDetalleCreateDto
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
