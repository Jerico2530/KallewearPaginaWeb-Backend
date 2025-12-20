/* 
 * Proyecto Empresarial – Controlador de Preguntas
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * preguntas dentro del sistema. Expone endpoints RESTful para
 * operaciones de lectura, creación, actualización, eliminación y
 * exportación de datos en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de preguntas.
 * - Consultar el detalle de una pregunta por su identificador.
 * - Crear nuevas preguntas.
 * - Actualizar preguntas de forma completa o parcial.
 * - Eliminar preguntas existentes.
 * - Exportar el listado de preguntas a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la gestión de solicitudes HTTP relacionadas con
 * el ciclo de vida de preguntas, delegando toda la lógica de
 * negocio al servicio correspondiente y asegurando la entrega
 * de respuestas estandarizadas mediante ApiResponse.
 *
 * Descripción general del código:
 * - Se inyectan el servicio de preguntas y el logger.
 * - Cada endpoint está protegido mediante permisos específicos.
 * - Las acciones llaman directamente a la capa de servicios.
 * - Las respuestas se devuelven con el código HTTP adecuado.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Pregunta;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntaController : ControllerBase
    {
        private readonly ILogger<PreguntaController> _logger;
        private readonly IPreguntaService _PreguntaService;

        public PreguntaController(ILogger<PreguntaController> logger, IPreguntaService PreguntaService)
        {
            _logger = logger;
            _PreguntaService = PreguntaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Pregunta.Ver")]

        public async Task<ActionResult<ApiResponse<List<PreguntaDto>>>> GetPregunta()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Preguntas");
            // Llama la capa de servicios para obtener el listado de preguntas.
            var response = await _PreguntaService.ObtenerTodosLosPreguntaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetPregunta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Pregunta.VerDetalle")]
        public async Task<ActionResult<ApiResponse<PreguntaDto>>> GetPregunta(int id)
        {
            _logger.LogInformation("🔍 Solicitando Pregunta con ID {PreguntaId}.", id);
            // Consulta al servicio por el detalle de la pregunta solicitada.
            var response = await _PreguntaService.ObtenerPreguntaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Pregunta.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _PreguntaService.ExportarExcelPreguntasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "preguntas.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Pregunta.Crear")]
        public async Task<ActionResult<ApiResponse<PreguntaDto>>> CrearPregunta([FromBody] PreguntaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Pregunta.");
            // Solicita la creación de la pregunta en la capa de servicios.
            var response = await _PreguntaService.CrearPreguntaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Pregunta: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetPregunta", new { id = carrito?.PreguntaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Pregunta.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarPregunta(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Pregunta con ID {PreguntaId}", id);
            // Solicita al servicio eliminar la pregunta indicada.
            var response = await _PreguntaService.EliminarPreguntaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Pregunta.Actualizar")]
        public async Task<ActionResult<ApiResponse<PreguntaDto>>> ActualizarPregunta(int id, [FromBody] PreguntaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando pregunta con ID {Id}.", id);
            // Solicita la actualización completa de la pregunta.
            var response = await _PreguntaService.ActualizarPreguntaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Pregunta.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<PreguntaDto>>> UpdateParcialPregunta(int id, [FromBody] JsonPatchDocument<PreguntaUpdateDto> patchDto)
        {
            

            _logger.LogInformation("🧩 Actualización parcial de Pregunta con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _PreguntaService.ActualizarParcialPreguntaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}