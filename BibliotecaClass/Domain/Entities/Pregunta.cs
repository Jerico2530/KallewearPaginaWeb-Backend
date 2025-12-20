using System.ComponentModel.DataAnnotations;

namespace BiblotecaWeb;

public class Pregunta
{
    [Key]
    public int PreguntaId { get; set; }
    public string Preguntas {  get; set; }
    public string Respuesta { get; set; }
    public bool Estado {  get; set; }
    public DateTime FechaRegistro { get; set; }= DateTime.Now;
}
