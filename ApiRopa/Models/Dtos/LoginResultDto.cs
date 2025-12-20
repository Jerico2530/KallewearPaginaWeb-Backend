namespace ApiRopa.Models.Dtos
{
    /// <summary>
    /// DTO que representa la respuesta de un login exitoso.
    /// Contiene el token JWT y la información básica del usuario.
    /// </summary>
    public class LoginResultDto
    {
        public string Token { get; set; }
        public UsuarioInfoDto Usuario { get; set; }
    }

    /// <summary>
    /// DTO que encapsula la información personal y permisos del usuario.
    /// </summary>
    public class UsuarioInfoDto
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }  
        public string ApellidoCompleto { get; set; }
        public string DNI { get; set; }
        public string CorreoElectronico { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permisos { get; set; }
    }
}
    