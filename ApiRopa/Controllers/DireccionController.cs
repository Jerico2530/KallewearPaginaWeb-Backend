/*
 * Proyecto Empresarial – Controlador de Direcciones
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * direcciones asociadas a usuarios dentro del sistema. Expone endpoints
 * RESTful que permiten realizar operaciones de consulta, creación,
 * actualización, eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de direcciones registradas.
 * - Consultar el detalle de una dirección específica.
 * - Crear, actualizar (total o parcial) y eliminar direcciones.
 * - Exportar la información en formato Excel para análisis externo.
 *
 * Propósito del componente:
 * Centralizar la gestión de peticiones HTTP relacionadas con las
 * direcciones del usuario, delegando la lógica de negocio al servicio 
 * correspondiente y asegurando respuestas consistentes mediante ApiResponse.
 *
 * Descripción general del código:
 * - El controlador recibe e inyecta el logger y el servicio de direcciones.
 * - Todos los endpoints están protegidos mediante permisos personalizados.
 * - Se emplea ApiResponse como estructura estándar para las respuestas.
 * - Cada acción retorna códigos HTTP apropiados según el resultado.
 */
using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Direccion;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DireccionController : ControllerBase
    {
        private readonly ILogger<DireccionController> _logger;
        private readonly IDireccionService _DireccionService;

        public DireccionController(ILogger<DireccionController> logger, IDireccionService DireccionService)
        {
            _logger = logger;
            _DireccionService = DireccionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Direccion.Ver")] 

        public async Task<ActionResult<ApiResponse<List<DireccionDto>>>> GetDireccion()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Direccions");
            // Llama la capa de servicios para obtener el listado de direccion.
            var response = await _DireccionService.ObtenerTodosLosDireccionAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetDireccion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Direccion.VerDetalle")] 
        public async Task<ActionResult<ApiResponse<DireccionDto>>> GetDireccion(int id)
        {
            _logger.LogInformation("🔍 Solicitando Direccion con ID {DireccionId}.", id);
            // Consulta al servicio por el detalle de la categoría solicitada.
            var response = await _DireccionService.ObtenerDireccionPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Direccion.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _DireccionService.ExportarExcelDireccionesAsync();

            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "direccion.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Direccion.Crear")] 
        public async Task<ActionResult<ApiResponse<DireccionDto>>> CrearDireccion([FromBody] DireccionCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Direccion.");
            // Solicita la creación de la direccion en la capa de servicios.
            var response = await _DireccionService.CrearDireccionAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Direccion: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetDireccion", new { id = carrito?.DireccionId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Direccion.Eliminar")] 
        public async Task<ActionResult<ApiResponse<object>>> EliminarDireccion(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Direccion con ID {DireccionId}", id);
            // Solicita al servicio eliminar la categoría indicada.
            var response = await _DireccionService.EliminarDireccionAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Direccion.Actualizar")]
        public async Task<ActionResult<ApiResponse<DireccionDto>>> ActualizarDireccion(int id, [FromBody] DireccionUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando categoría con ID {Id}.", id);
            // Solicita la actualización completa de la categoría.
            var response = await _DireccionService.ActualizarDireccionAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Direccion.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<DireccionDto>>> UpdateParcialDireccion(int id, [FromBody] JsonPatchDocument<DireccionUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Direccion con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _DireccionService.ActualizarParcialDireccionAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
