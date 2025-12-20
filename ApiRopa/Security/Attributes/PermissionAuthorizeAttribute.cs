using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiRopa.Security.Attributes
{

    /// <summary>
    /// Filtro de autorización para validar que el usuario autenticado
    /// posea un permiso específico dentro de sus claims.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AutorizacionPermiso : Attribute, IAuthorizationFilter
    {
        private readonly string _permiso;

        /// <summary>
        /// Inicializa el atributo con el permiso requerido.
        /// </summary>
        /// <param name="permiso">Nombre del permiso que se debe validar.</param>
        public AutorizacionPermiso(string permiso)
        {
            _permiso = permiso;
        }

        /// <summary>
        /// Ejecuta la validación de permisos antes de que el endpoint sea procesado.
        /// </summary>
        /// <param name="context">Contexto de autorización con la información del usuario.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // Si el usuario no está autenticado, se bloquea el acceso inmediatamente.
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Verifica si el usuario tiene el permiso requerido dentro de los claims.  
            var haspermiso = user.Claims.Any(c =>
                c.Type == "permiso" &&
                string.Equals(c.Value, _permiso, StringComparison.OrdinalIgnoreCase)
            );

            // Si no tiene el permiso, se retorna respuesta de acceso denegado.
            if (!haspermiso)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
