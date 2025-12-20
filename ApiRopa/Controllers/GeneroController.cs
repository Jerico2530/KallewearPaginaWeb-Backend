/*
 * Proyecto Empresarial – Controlador de Géneros
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de administrar
 * los géneros disponibles dentro del sistema. Expone endpoints RESTful
 * para consultar, crear, actualizar, eliminar y exportar información.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de géneros.
 * - Consultar el detalle de un género específico.
 * - Crear, actualizar (total o parcial) y eliminar géneros.
 * - Exportar los registros a un archivo Excel.
 *
 * Propósito del componente:
 * Gestionar todas las solicitudes HTTP relacionadas con el ciclo de vida
 * de los géneros, delegando la lógica de negocio al servicio correspondiente
 * y asegurando respuestas consistentes mediante ApiResponse.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de géneros.
 * - Cada endpoint utiliza permisos específicos mediante atributos de seguridad.
 * - Todos los métodos retornan respuestas estandarizadas con ApiResponse.
 * - Se aplican códigos HTTP adecuados según el resultado de cada operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Genero;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        private readonly ILogger<GeneroController> _logger;
        private readonly IGeneroService _GeneroService;

        public GeneroController(ILogger<GeneroController> logger, IGeneroService GeneroService)
        {
            _logger = logger;
            _GeneroService = GeneroService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Genero.Ver")]

        public async Task<ActionResult<ApiResponse<List<GeneroDto>>>> GetGenero()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Generos");
            // Llama la capa de servicios para obtener el listado de genero.
            var response = await _GeneroService.ObtenerTodosLosGeneroAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetGenero")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Genero.VerDetalle")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> GetGenero(int id)
        {
            _logger.LogInformation("🔍 Solicitando Genero con ID {GeneroId}.", id);
            // Consulta al servicio por el detalle de la genero solicitada.
            var response = await _GeneroService.ObtenerGeneroPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Genero.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _GeneroService.ExportarExcelGenerosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "genero.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Genero.Crear")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> CrearGenero([FromBody] GeneroCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Genero.");
            // Solicita la creación de la genero en la capa de servicios.
            var response = await _GeneroService.CrearGeneroAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Genero: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetGenero", new { id = carrito?.GeneroId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Genero.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarGenero(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Genero con ID {GeneroId}", id);
            // Solicita al servicio eliminar la genero indicada.
            var response = await _GeneroService.EliminarGeneroAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Genero.Actualizar")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> ActualizarGenero(int id, [FromBody] GeneroUpdateDto updateDto)
        {

            _logger.LogInformation("🔄 Actualizando genero con ID {Id}.", id);
            // Solicita la actualización completa de la genero.
            var response = await _GeneroService.ActualizarGeneroAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Genero.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<GeneroDto>>> UpdateParcialGenero(int id, [FromBody] JsonPatchDocument<GeneroUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Genero con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _GeneroService.ActualizarParcialGeneroAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}