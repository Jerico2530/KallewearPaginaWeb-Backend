/* 
 * Proyecto Empresarial – Contenedor de Testimonios
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * los testimonios del sistema. Expone endpoints RESTful para realizar
 * operaciones de consulta, creación, actualización, eliminación y 
 * exportación de información en formatos estructurados.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de testimonios.
 * - Consultar el detalle de un testimonio específico.
 * - Crear nuevos testimonios.
 * - Actualizar testimonios de manera total o parcial.
 * - Eliminar testimonios.
 * - Exportar los testimonios disponibles a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar y orquestar todas las solicitudes HTTP relacionadas
 * al ciclo de vida de los testimonios, delegando la lógica de negocio
 * al servicio correspondiente. Garantiza consistencia en validaciones,
 * respuestas y códigos de estado HTTP.
 *
 * Descripción general del código:
 * - Se inyectan el logger y el servicio especializado de testimonios.
 * - Cada endpoint está protegido por permisos definidos mediante atributos.
 * - Se utiliza ApiResponse como formato estándar de salida.
 * - Los métodos devuelven códigos HTTP coherentes según el resultado.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using ApiRopa.Services;
using ApiRopa.Services.IServices;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Testimonio;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestimonioController : ControllerBase
    {
        private readonly ILogger<TestimonioController> _logger;
        private readonly ITestimonioService _TestimonioService;

        public TestimonioController(ILogger<TestimonioController> logger, ITestimonioService TestimonioService)
        {
            _logger = logger;
            _TestimonioService = TestimonioService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Testimonio.Ver")]

        public async Task<ActionResult<ApiResponse<List<TestimonioDto>>>> GetTestimoniol()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Testimonios");
            // Llama la capa de servicios para obtener el listado de testimonios
            var response = await _TestimonioService.ObtenerTodosLosTestimonioAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("{id:int}", Name = "GetTestimonio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Testimonio.Ver")]

        public async Task<ActionResult<ApiResponse<TestimonioDto>>> GetTestimonio(int id)
        {
            _logger.LogInformation("🔍 Solicitando Testimonio con ID {TestimonioId}.", id);
            // Consulta al servicio por el detalle de la testimonio solicitada.
            var response = await _TestimonioService.ObtenerTestimonioPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Testimonio.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _TestimonioService.ExportarExcelTestimoniosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "testimonio.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Testimonio.Crear")]

        public async Task<ActionResult<ApiResponse<TestimonioDto>>> CrearTestimoniol([FromBody] TestimonioCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Testimonio.");
            // Solicita la creación de la testimonio en la capa de servicios
            var response = await _TestimonioService.CrearTestimonioAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Testimonio: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetTestimonio", new { id = carrito?.TestimonioId }, response);

        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Testimonio.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarTestimonio(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Testimonio con ID {TestimonioId}", id);
            // Solicita al servicio eliminar la testimonio indicada.
            var response = await _TestimonioService.EliminarTestimonioAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Testimonio.Actualizar")]
        public async Task<ActionResult<ApiResponse<TestimonioDto>>> ActualizarTestimonio(int id, [FromBody] TestimonioUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando testimonio con ID {Id}.", id);
            // Solicita la actualización completa de la testimonio.
            var response = await _TestimonioService.ActualizarTestimonioAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }




        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Testimonio.ActualizarParcial")]


        public async Task<ActionResult<ApiResponse<TestimonioDto>>> UpdateParcialTestimonio(int id, JsonPatchDocument<TestimonioUpdateDto> patchDto)
        {
            
            _logger.LogInformation("🧩 Actualización parcial de Testimonio con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _TestimonioService.ActualizarParcialTestimonioAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
