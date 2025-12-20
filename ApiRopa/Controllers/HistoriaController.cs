/* 
 * Proyecto Empresarial – Controlador de Historias
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las historias dentro del sistema. Expone endpoints RESTful para
 * operaciones de consulta, creación, actualización (completa o parcial),
 * eliminación y exportación en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de historias.
 * - Consultar el detalle de una historia específica.
 * - Crear, actualizar y eliminar historias.
 * - Exportar información a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la gestión de solicitudes HTTP relacionadas al ciclo
 * de vida de las historias, delegando la lógica de negocio al servicio
 * correspondiente y administrando validaciones, códigos de estado y
 * manejo de errores.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de historias.
 * - Cada endpoint implementa permisos específicos mediante atributos
 *   de autorización personalizada.
 * - Todas las respuestas siguen un formato estándar mediante ApiResponse.
 * - Se retornan códigos HTTP coherentes según el resultado de cada operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Historia;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriaController : ControllerBase
    {
        private readonly ILogger<HistoriaController> _logger;
        private readonly IHistoriaService _HistoriaService;

        public HistoriaController(ILogger<HistoriaController> logger, IHistoriaService HistoriaService)
        {
            _logger = logger;
            _HistoriaService = HistoriaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Historia.Ver")]

        public async Task<ActionResult<ApiResponse<List<HistoriaDto>>>> GetHistoria()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Historias");
            // Llama la capa de servicios para obtener el listado de historias.
            var response = await _HistoriaService.ObtenerTodosLosHistoriaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetHistoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Historia.VerDetalle")]
        public async Task<ActionResult<ApiResponse<HistoriaDto>>> GetHistoria(int id)
        {
            _logger.LogInformation("🔍 Solicitando Historia con ID {HistoriaId}.", id);
            // Consulta al servicio por el detalle de la historia solicitada.
            var response = await _HistoriaService.ObtenerHistoriaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Historia.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _HistoriaService.ExportarExcelHistoriasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "historia.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Historia.Crear")]
        public async Task<ActionResult<ApiResponse<HistoriaDto>>> CrearHistoria([FromBody] HistoriaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Historia.");
            // Solicita la creación de la historia en la capa de servicios.
            var response = await _HistoriaService.CrearHistoriaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Historia: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetHistoria", new { id = carrito?.HistoriaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Historia.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarHistoria(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Historia con ID {HistoriaId}", id);
            // Solicita al servicio eliminar la historia indicada.
            var response = await _HistoriaService.EliminarHistoriaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Historia.Actualizar")]
        public async Task<ActionResult<ApiResponse<HistoriaDto>>> ActualizarHistoria(int id, [FromBody] HistoriaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando historia con ID {Id}.", id);
            // Solicita la actualización completa de la historia.
            var response = await _HistoriaService.ActualizarHistoriaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Historia.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<HistoriaDto>>> UpdateParcialHistoria(int id, [FromBody] JsonPatchDocument<HistoriaUpdateDto> patchDto)
        {
           
            _logger.LogInformation("🧩 Actualización parcial de Historia con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _HistoriaService.ActualizarParcialHistoriaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}