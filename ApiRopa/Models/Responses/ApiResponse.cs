using System.Net;

namespace ApiRopa.Models.Responses
{
    /// <summary>
    /// Clase genérica para envolver las respuestas de la API.
    /// Incluye estado de éxito, datos, mensajes y errores.
    /// </summary>
    /// <typeparam name="T">Tipo de los datos que se retornan en la respuesta.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Datos devueltos por la API. Puede ser nulo en caso de error.
        /// </summary>
        public T Resultado { get; set; }

        /// <summary>
        /// Código HTTP asociado a la respuesta.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Indica si la operación fue exitosa.
        /// Por defecto es true.
        /// </summary>
        public bool IsExitoso { get; set; } = true;

        /// <summary>
        /// Lista de errores ocurridos durante la operación.
        /// Normalmente se llena en caso de fallo.
        /// </summary>
        public List<ErrorDetail> ErroresMessages { get; set; } = new();

        /// <summary>
        /// Mensaje general de la respuesta.
        /// Puede ser un mensaje de éxito o de información adicional.
        /// </summary>
        public string Mensaje { get; set; }
    }

    /// <summary>
    /// Clase que representa el detalle de un error en la API.
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Nombre del campo o propiedad relacionado con el error.
        /// </summary>
        public string Campo { get; set; }

        /// <summary>
        /// Mensaje descriptivo del error ocurrido.
        /// </summary>
        public string Mensaje { get; set; }
    }
}
