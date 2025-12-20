using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class GeneroDto
{
    [Key]
    [Display(Name = "ID Género")]
    public int GeneroId { get; set; }
    public string Tipo { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
