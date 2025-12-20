/* 
 * Proyecto Empresarial – Controlador de Permisos por Rol (PermRol)
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las relaciones entre roles y permisos dentro del sistema. Expone
 * endpoints RESTful para la lectura, creación, actualización,
 * eliminación y exportación de estas asociaciones.
 *
 * Funcionalidades clave:
 * - Obtener todas las relaciones PermRol con información detallada.
 * - Consultar un PermRol específico por su identificador.
 * - Crear nuevas asociaciones entre roles y permisos.
 * - Actualizar o eliminar registros existentes.
 * - Exportar los PermRol a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la gestión de peticiones HTTP relacionadas a la
 * administración de permisos asignados a roles. Este controlador
 * delega la lógica de negocio al servicio correspondiente y se
 * encarga de responder con códigos HTTP adecuados y estructura
 * ApiResponse.
 *
 * Descripción general del código:
 * - Se inyectan las dependencias de logging y del servicio PermRol.
 * - Cada acción está protegida mediante permisos específicos.
 * - Se utiliza ApiResponse como contrato estándar de respuesta.
 * - Cada endpoint delega la operación a la capa de servicios.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.PermRol;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermRolController : ControllerBase
    {
        private readonly ILogger<PermRolController> _logger;
        private readonly IPermRolService _PermRolService;

        public PermRolController(ILogger<PermRolController> logger, IPermRolService PermRolService)
        {
            _logger = logger;
            _PermRolService = PermRolService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("PermRol.Ver")]
        public async Task<ActionResult<ApiResponse<List<PermRolDto>>>> GetPermRol()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los PermRols");
            // Llama la capa de servicios para obtener el listado de permRols.
            var response = await _PermRolService.ObtenerPermRolConDetallesAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetPermRol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("PermRol.VerDetalle")]

        public async Task<ActionResult<ApiResponse<PermRolDto>>> GetPermRol(int id)
        {
            _logger.LogInformation("🔍 Solicitando PermRol con ID {PermRolId}.", id);
            // Consulta al servicio por el detalle de la permRol solicitada.
            var response = await _PermRolService.ObtenerPermRolPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);


        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("PermRol.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _PermRolService.ExportarExcelPermRolesAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "permRol.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("PermRol.Crear")]
        public async Task<ActionResult<ApiResponse<PermRolDto>>> CrearPermRol([FromBody] PermRolCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo PermRol.");
            // Solicita la creación de la permRol en la capa de servicios.
            var response = await _PermRolService.CrearPermRolAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear PermRol: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetPermRol", new { id = carrito?.PermRolId }, response);
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("PermRol.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarPermRol(int id)
        {
            _logger.LogInformation("Iniciando eliminación del PermRol con ID {PermRolId}", id);
            // Solicita al servicio eliminar la permRol indicada.
            var response = await _PermRolService.EliminarPermRolAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("PermRol.Actualizar")]
        public async Task<ActionResult<ApiResponse<PermRolDto>>> ActualizarPermRol(int id, [FromBody] PermRolUpdateDto updateDto)
        {

            _logger.LogInformation("🔄 Actualizando permRol con ID {Id}.", id);
            // Solicita la actualización completa de la permRol.
            var response = await _PermRolService.ActualizarPermRolAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


    }
}
