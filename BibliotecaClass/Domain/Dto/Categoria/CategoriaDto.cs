namespace BiblotecaWeb;
using System.ComponentModel.DataAnnotations;
public class CategoriaDto
{
    [Display(Name = "ID Categoria")]
    public int CategoriaId { get; set; }
    [Display(Name = "Categoria")]
    public string DesCategoria { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
