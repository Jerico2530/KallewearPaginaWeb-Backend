/*
 * Proyecto Empresarial – Controlador de Monedas
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las monedas utilizadas dentro del sistema. Expone endpoints RESTful
 * para operaciones de consulta, creación, actualización, eliminación
 * y exportación de información.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de monedas.
 * - Consultar el detalle de una moneda específica.
 * - Crear nuevas monedas y actualizar o eliminar registros existentes.
 * - Exportar el listado de monedas a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la administración de todas las operaciones HTTP relacionadas
 * a las monedas, delegando la lógica de negocio al servicio correspondiente.
 * El controlador gestiona validaciones, respuestas estructuradas y códigos
 * de estado, asegurando consistencia en toda la API.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de monedas.
 * - Los endpoints están protegidos por permisos específicos mediante atributos.
 * - Las respuestas utilizan ApiResponse como estructura estándar.
 * - Cada acción retorna códigos HTTP adecuados según el resultado de la operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Moneda;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonedaController : ControllerBase
    {
        private readonly ILogger<MonedaController> _logger;
        private readonly IMonedaService _MonedaService;

        public MonedaController(ILogger<MonedaController> logger, IMonedaService MonedaService)
        {
            _logger = logger;
            _MonedaService = MonedaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Moneda.Ver")]

        public async Task<ActionResult<ApiResponse<List<MonedaDto>>>> GetMoneda()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Monedas");
            // Llama la capa de servicios para obtener el listado de monedas.
            var response = await _MonedaService.ObtenerTodosLosMonedaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetMoneda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Moneda.VerDetalle")]

        public async Task<ActionResult<ApiResponse<MonedaDto>>> GetMoneda(int id)
        {
            _logger.LogInformation("🔍 Solicitando Moneda con ID {MonedaId}.", id);
            // Consulta al servicio por el detalle de la moneda solicitada.
            var response = await _MonedaService.ObtenerMonedaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Moneda.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _MonedaService.ExportarExcelMonedasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "moneda.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Moneda.Crear")]
        public async Task<ActionResult<ApiResponse<MonedaDto>>> CrearMoneda([FromBody] MonedaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Moneda.");
            // Solicita la creación de la moneda en la capa de servicios.
            var response = await _MonedaService.CrearMonedaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Moneda: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetMoneda", new { id = carrito?.MonedaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Moneda.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarMoneda(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Moneda con ID {MonedaId}", id);
            // Solicita al servicio eliminar la moneda indicada.
            var response = await _MonedaService.EliminarMonedaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Moneda.Actualizar")]
        public async Task<ActionResult<ApiResponse<MonedaDto>>> ActualizarMoneda(int id, [FromBody] MonedaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando moneda con ID {Id}.", id);
            // Solicita la actualización completa de la moneda.
            var response = await _MonedaService.ActualizarMonedaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Moneda.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<MonedaDto>>> UpdateParcialMoneda(int id, [FromBody] JsonPatchDocument<MonedaUpdateDto> patchDto)
        {
 
            _logger.LogInformation("🧩 Actualización parcial de Moneda con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _MonedaService.ActualizarParcialMonedaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}