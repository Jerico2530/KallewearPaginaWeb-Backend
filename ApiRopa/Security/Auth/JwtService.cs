using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Usuario;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiRopa.Security.Auth
{
    /// <summary>
    /// Servicio responsable de generar tokens JWT tanto para usuarios registrados
    /// como invitados dentro del sistema.
    /// </summary>
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public JwtService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Genera un JWT para un usuario registrado,
        /// incluyendo sus roles y permisos activos.
        /// </summary>
        /// <param name="usuarioId">Identificador del usuario.</param>
        /// <returns>Token JWT firmado y serializado.</returns>
        public string GenerarJWT(int usuarioId)
        {
            // Se obtiene el usuario junto con sus roles y permisos
            var usuario = _context.Usuarios
                     .Include(u => u.UserRoles)         
                         .ThenInclude(ur => ur.Rol)      
                         .ThenInclude(r => r.PermRoles)
                         .ThenInclude(rp => rp.Permiso)
                     .FirstOrDefault(u => u.UsuarioId == usuarioId); 

            if (usuario == null)
                throw new Exception("Usuario no encontrado");

            // Claims base del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()), 
                new Claim(ClaimTypes.Email, usuario.CorreoElectronico ?? ""),       
            };

            // Agrega todos los roles activos del usuario
            foreach (var userRol in usuario.UserRoles.Where(ur => ur.Estado && ur.Rol.Estado))
            {
                claims.Add(new Claim(ClaimTypes.Role, userRol.Rol.NombreRol)); 

            }

            // Agrega permisos activos del usuario (sin duplicar)
            var permisos = usuario.UserRoles
                .SelectMany(ur => ur.Rol.PermRoles)
                .Where(rp => rp.Estado && rp.Permiso.Estado)
                .Select(rp => rp.Permiso.NombrePermiso)
                .Distinct();

            foreach (var permiso in permisos)
            {
                claims.Add(new Claim("permiso", permiso));
            }

            // Configuración de firma y creación del token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],       
                audience: _config["Jwt:Audience"],   
                claims: claims,                             
                expires: DateTime.UtcNow.AddHours(1),       
                signingCredentials: credentials             
            );

            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Genera un JWT temporal para usuarios invitados sin registro.
        /// Incluye roles y permisos definidos dinámicamente.
        /// </summary>
        /// <param name="invitado">Información del invitado autenticado.</param>
        /// <returns>Token JWT firmado y válido por un tiempo limitado.</returns>
        public string GenerarJWTParaInvitado(AuthResponseDto invitado)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, invitado.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, invitado.NombreCompleto ?? "Invitado"),
                new Claim("tipoUsuario", "Invitado")
            };

            // Rol asignado al invitado
            foreach (var rol in invitado.Roles)
                claims.Add(new Claim(ClaimTypes.Role, rol));

            // Permisos asignados al invitado
            foreach (var permiso in invitado.Permisos)
                claims.Add(new Claim("permiso", permiso));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
