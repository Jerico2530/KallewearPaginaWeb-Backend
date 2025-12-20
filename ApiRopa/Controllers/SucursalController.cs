/* 
 * Proyecto Empresarial – Controlador de Sucursales
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las sucursales del sistema. Expone endpoints RESTful para realizar
 * operaciones de consulta, creación, actualización, eliminación y 
 * exportación de datos en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de sucursales.
 * - Obtener el detalle de una sucursal específica.
 * - Crear, actualizar (total o parcial) y eliminar sucursales.
 * - Exportar el catálogo de sucursales a un archivo Excel.
 *
 * Propósito del componente:
 * Orquestar las solicitudes HTTP relacionadas al ciclo de vida de las
 * sucursales, delegando la lógica a la capa de servicios y gestionando
 * validaciones, respuestas y códigos de estado de manera estandarizada.
 *
 * Descripción general del código:
 * - Se inyectan el logger y el servicio de sucursales.
 * - Cada endpoint cuenta con permisos específicos mediante atributos personalizados.
 * - Se utiliza ApiResponse como estructura de respuesta unificada.
 * - Las acciones retornan códigos HTTP adecuados basados en el resultado del servicio.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Dto.Sucursal;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SucursalController : ControllerBase
    {
        private readonly ILogger<SucursalController> _logger;
        private readonly ISucursalService _SucursalService;

        public SucursalController(ILogger<SucursalController> logger, ISucursalService SucursalService)
        {
            _logger = logger;
            _SucursalService = SucursalService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Sucursal.Ver")]

        public async Task<ActionResult<ApiResponse<List<SucursalDto>>>> GetSucursal()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Sucursals");
            // Llama la capa de servicios para obtener el listado de sucursals.
            var response = await _SucursalService.ObtenerTodosLosSucursalAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetSucursal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Sucursal.VerDetalle")]
        public async Task<ActionResult<ApiResponse<SucursalDto>>> GetSucursal(int id)
        {
            _logger.LogInformation("🔍 Solicitando Sucursal con ID {SucursalId}.", id);
            // Consulta al servicio por el detalle de la sucursal solicitada.
            var response = await _SucursalService.ObtenerSucursalPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Sucursal.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _SucursalService.ExportarExcelSucursalesAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "sucursal.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Sucursal.Crear")]
        public async Task<ActionResult<ApiResponse<SucursalDto>>> CrearSucursal([FromBody] SucursalCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Sucursal.");
            // Solicita la creación de la sucursal en la capa de servicios.
            var response = await _SucursalService.CrearSucursalAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Sucursal: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetSucursal", new { id = carrito?.SucursalId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Sucursal.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarSucursal(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Sucursal con ID {SucursalId}", id);
            // Solicita al servicio eliminar la sucursal indicada.
            var response = await _SucursalService.EliminarSucursalAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Sucursal.Actualizar")]
        public async Task<ActionResult<ApiResponse<SucursalDto>>> ActualizarSucursal(int id, [FromBody] SucursalUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando sucursal con ID {Id}.", id);
            // Solicita la actualización completa de la sucursal
            var response = await _SucursalService.ActualizarSucursalAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Sucursal.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<SucursalDto>>> UpdateParcialSucursal(int id, [FromBody] JsonPatchDocument<SucursalUpdateDto> patchDto)
        {
            
            _logger.LogInformation("🧩 Actualización parcial de Sucursal con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _SucursalService.ActualizarParcialSucursalAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }
    }
}