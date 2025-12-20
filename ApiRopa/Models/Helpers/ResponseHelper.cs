using ApiRopa.Models.Responses;
using FluentValidation.Results;
using System.Net;

namespace ApiRopa.Models
{
    /// <summary>
    /// Clase auxiliar estática para generar respuestas estándar de la API.
    /// Facilita la creación de respuestas exitosas y de error de manera consistente.
    /// </summary>
    public static class ResponseHelper
    {
        /// <summary>
        /// Genera una respuesta exitosa con datos y mensaje opcional.
        /// </summary>
        /// <typeparam name="T">Tipo de datos que se devuelven.</typeparam>
        /// <param name="data">Datos de la respuesta.</param>
        /// <param name="mensaje">Mensaje adicional opcional.</param>
        /// <param name="code">Código HTTP de la respuesta (por defecto 200 OK).</param>
        /// <returns>ApiResponse con estado exitoso.</returns>
        public static ApiResponse<T> Success<T>(T data, string mensaje = null, HttpStatusCode code = HttpStatusCode.OK)
            => new()
            {
                Resultado = data,
                IsExitoso = true,
                StatusCode = code,
                Mensaje = mensaje
            };

        /// <summary>
        /// Genera una respuesta de error con un solo mensaje de error.
        /// </summary>
        /// <typeparam name="T">Tipo de datos que se devolverían en caso de éxito.</typeparam>
        /// <param name="mensaje">Mensaje de error.</param>
        /// <param name="campo">Nombre del campo relacionado con el error (opcional).</param>
        /// <param name="code">Código HTTP de la respuesta (por defecto 400 Bad Request).</param>
        /// <returns>ApiResponse con estado de error.</returns>
        public static ApiResponse<T> Fail<T>(string mensaje, string campo = null, HttpStatusCode code = HttpStatusCode.BadRequest)
            => new()
            {
                IsExitoso = false,
                StatusCode = code,
                ErroresMessages = new List<ErrorDetail> { new() { Campo = campo, Mensaje = mensaje } }
            };

        /// <summary>
        /// Genera una respuesta de error con múltiples errores personalizados.
        /// </summary>
        /// <typeparam name="T">Tipo de datos que se devolverían en caso de éxito.</typeparam>
        /// <param name="errores">Lista de detalles de error.</param>
        /// <param name="code">Código HTTP de la respuesta (por defecto 400 Bad Request).</param>
        /// <returns>ApiResponse con estado de error.</returns>
        public static ApiResponse<T> Fail<T>(List<ErrorDetail> errores, HttpStatusCode code = HttpStatusCode.BadRequest)
            => new()
            {
                IsExitoso = false,
                StatusCode = code,
                ErroresMessages = errores
            };

        /// <summary>
        /// Genera una respuesta de error a partir de errores de validación de FluentValidation.
        /// </summary>
        /// <typeparam name="T">Tipo de datos que se devolverían en caso de éxito.</typeparam>
        /// <param name="validationErrors">Lista de errores de validación.</param>
        /// <param name="code">Código HTTP de la respuesta (por defecto 400 Bad Request).</param>
        /// <returns>ApiResponse con estado de error.</returns>
        public static ApiResponse<T> Fail<T>(IList<ValidationFailure> validationErrors, HttpStatusCode code = HttpStatusCode.BadRequest)
        {
            var errores = validationErrors
                .Select(e => new ErrorDetail { Campo = e.PropertyName, Mensaje = e.ErrorMessage })
                .ToList();
            return Fail<T>(errores, code);
        }

        /// <summary>
        /// Genera una respuesta de error a partir de una excepción capturada.
        /// Útil para manejo profesional de errores internos (500 Internal Server Error).
        /// </summary>
        /// <typeparam name="T">Tipo de datos que se devolverían en caso de éxito.</typeparam>
        /// <param name="ex">Excepción capturada.</param>
        /// <param name="code">Código HTTP de la respuesta (por defecto 500 Internal Server Error).</param>
        /// <returns>ApiResponse con estado de error y detalle de la excepción.</returns>
        public static ApiResponse<T> FailException<T>(Exception ex, HttpStatusCode code = HttpStatusCode.InternalServerError)
        {
            var errores = new List<ErrorDetail>
            {
                new() { Campo = "Exception", Mensaje = ex.Message }
            };
            return Fail<T>(errores, code);
        }
    }
}
