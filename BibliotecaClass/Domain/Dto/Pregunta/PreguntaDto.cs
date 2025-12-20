namespace BiblotecaWeb;
using System.ComponentModel.DataAnnotations;

public class PreguntaDto
{
    [Display(Name = "ID Pregunta")]
    public int PreguntaId { get; set; }
    [Display(Name = "Pregunta")]
    public string Preguntas { get; set; }
    [Display(Name = "Respuesta")]
    public string Respuesta { get; set; }
    public bool Estado { get; set; }
    [Display(Name = "Fecha de Registro")]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
