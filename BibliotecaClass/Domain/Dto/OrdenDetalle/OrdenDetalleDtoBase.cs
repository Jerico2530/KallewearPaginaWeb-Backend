namespace BiblotecaWeb.Domain.Dto.OrdenDetalle;
    using BiblotecaWeb.Domain.Entities;

    public class OrdenDetalleDtoBase
    {
        public int Cantidad { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public Orden Orden { get; set; }
        public int OrdenDetalleId { get; set; }
        public int OrdenId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public Producto Producto { get; set; }
        public int ProductoId { get; set; }
        public decimal SubTotal { get; set; }
    }
