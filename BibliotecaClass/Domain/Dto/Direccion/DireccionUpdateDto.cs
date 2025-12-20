namespace BiblotecaWeb;

public class DireccionUpdateDto
{
    public int UsuarioId { get; set; }
    public string Departamento { get; set; }
    public string Provincia { get; set; }
    public string Distrito { get; set; }
    public string Via { get; set; }
    public string Numero { get; set; }
    public bool Estado { get; set; }
}
