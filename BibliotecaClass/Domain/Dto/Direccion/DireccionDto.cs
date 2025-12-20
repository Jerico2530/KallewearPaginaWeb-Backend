namespace BiblotecaWeb.Domain.Dto.Direccion;
using System.ComponentModel.DataAnnotations;
public class DireccionDto
{
    [Display(Name = "ID Direccion")]
    public int DireccionId { get; set; }
    [Display(Name = "ID Usuario")]
    public int UsuarioId { get; set; }
    public string Departamento { get; set; }
    public string Provincia { get; set; }
    public string Distrito { get; set; }
    public string Via { get; set; }
    public string Numero { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
