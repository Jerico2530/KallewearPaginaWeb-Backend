/* 
 * Proyecto Empresarial – Controlador de Tallas
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * las tallas registradas en el sistema. Expone endpoints RESTful para 
 * operaciones de consulta, creación, actualización, eliminación y 
 * exportación de datos en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de tallas.
 * - Consultar el detalle de una talla específica.
 * - Crear, actualizar (total o parcial) y eliminar tallas.
 * - Exportar el catálogo de tallas a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar y coordinar todas las solicitudes HTTP relacionadas
 * al ciclo de vida de las tallas, delegando la lógica de negocio a la
 * capa de servicios y administrando respuestas, validaciones y
 * códigos de estado estandarizados.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de tallas.
 * - Cada endpoint está protegido mediante permisos específicos.
 * - Se emplea ApiResponse como estructura de respuesta uniforme.
 * - Las acciones retornan códigos HTTP adecuados según cada resultado.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Dto.Talla;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TallaController : ControllerBase
    {
        private readonly ILogger<TallaController> _logger;
        private readonly ITallaService _TallaService;

        public TallaController(ILogger<TallaController> logger, ITallaService TallaService)
        {
            _logger = logger;
            _TallaService = TallaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Talla.Ver")]
        public async Task<ActionResult<ApiResponse<List<TallaDto>>>> GetTalla()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Tallas");
            // Llama la capa de servicios para obtener el listado de tallas.
            var response = await _TallaService.ObtenerTodosLosTallaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetTalla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Talla.VerDetalle")]
        public async Task<ActionResult<ApiResponse<TallaDto>>> GetTalla(int id)
        {
            _logger.LogInformation("🔍 Solicitando Talla con ID {TallaId}.", id);
            // Consulta al servicio por el detalle de la talla solicitada.
            var response = await _TallaService.ObtenerTallaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Talla.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _TallaService.ExportarExcelTallasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "talla.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Talla.Crear")]
        public async Task<ActionResult<ApiResponse<TallaDto>>> CrearTalla([FromBody] TallaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Talla.");
            // Solicita la creación de la talla en la capa de servicios.
            var response = await _TallaService.CrearTallaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Talla: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetTalla", new { id = carrito?.TallaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Talla.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarTalla(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Talla con ID {TallaId}", id);
            // Solicita al servicio eliminar la talla indicada.
            var response = await _TallaService.EliminarTallaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Talla.Actualizar")]
        public async Task<ActionResult<ApiResponse<TallaDto>>> ActualizarTalla(int id, [FromBody] TallaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando talla con ID {Id}.", id);
            // Solicita la actualización completa de la talla.
            var response = await _TallaService.ActualizarTallaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Talla.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<TallaDto>>> UpdateParcialTalla(int id, [FromBody] JsonPatchDocument<TallaUpdateDto> patchDto)
        {
           
            _logger.LogInformation("🧩 Actualización parcial de Talla con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _TallaService.ActualizarParcialTallaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
