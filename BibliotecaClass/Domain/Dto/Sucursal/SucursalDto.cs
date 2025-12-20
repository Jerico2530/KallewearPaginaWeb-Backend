namespace BiblotecaWeb.Domain.Dto.Sucursal;
using System.ComponentModel.DataAnnotations;

public class SucursalDto
{
    [Display(Name = "ID Sucursal")]
    public int SucursalId { get; set; }
    public string Locales { get; set; }
    public string Descripcion { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
