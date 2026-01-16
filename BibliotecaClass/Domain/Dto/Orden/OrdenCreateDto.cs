using BiblotecaWeb.Model;

namespace BiblotecaWeb.Domain.Dto.Orden;

public class OrdenCreateDto
{

    public int UsuarioId { get; set; }
    public int? SucursalId { get; set; }
    public string MetodoEntrega { get; set; }
    public int? DireccionId { get; set; }
    public bool? Estado { get; set; }
    public decimal? Total { get; set; }



}
