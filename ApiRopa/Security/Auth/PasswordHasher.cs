namespace ApiRopa.Security.Auth
{
    /// <summary>
    /// Servicio responsable del hashing y validación de contraseñas
    /// utilizando el algoritmo BCrypt para mayor seguridad.
    /// </summary>
    public class PasswordHasher
    {
        /// <summary>
        /// Genera un hash seguro a partir de una contraseña en texto plano.
        /// BCrypt incorpora automáticamente un "salt" único por hash.
        /// </summary>
        /// <param name="password">Contraseña original en texto plano.</param>
        /// <returns>Cadena que contiene el hash generado con BCrypt.</returns>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifica si la contraseña ingresada coincide con el hash almacenado.
        /// </summary>
        /// <param name="passwordIngresado">Contraseña proporcionada por el usuario.</param>
        /// <param name="passwordHashAlmacenado">Hash previamente almacenado en base de datos.</param>
        /// <returns>True si la contraseña es válida, False en caso contrario.</returns>
        public bool VerificarPassword(string passwordIngresado, string passwordHashAlmacenado)
        {
            return BCrypt.Net.BCrypt.Verify(passwordIngresado, passwordHashAlmacenado);
        }
    }
}
