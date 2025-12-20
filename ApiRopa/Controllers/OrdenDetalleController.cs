/* 
 * Proyecto Empresarial – Controlador de Ordenes Detalle
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * los detalles de órdenes dentro del sistema. Expone endpoints RESTful 
 * para operaciones de lectura, creación, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - Obtener la lista completa de detalles de orden.
 * - Consultar información específica de un detalle de orden.
 * - Registrar nuevos detalles asociados a una orden.
 * - Actualizar información (total o parcial) de un detalle existente.
 * - Eliminar registros de detalles de una orden.
 *
 * Propósito del componente:
 * Centralizar la recepción de solicitudes HTTP vinculadas
 * al ciclo de vida de los detalles de las órdenes, delegando la lógica 
 * de negocio a la capa de servicios y administrando códigos de estado,
 * validaciones y respuestas unificadas.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de detalles de orden.
 * - Cada endpoint está protegido mediante permisos específicos.
 * - Todas las acciones retornan ApiResponse como contrato estándar.
 * - Las operaciones aplican códigos HTTP adecuados según el resultado.
 */
using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.OrdenDetalle;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenDetalleController : ControllerBase
    {
        private readonly ILogger<OrdenDetalleController> _logger;
        private readonly IOrdenDetalleService _OrdenDetalleService;

        public OrdenDetalleController(ILogger<OrdenDetalleController> logger, IOrdenDetalleService OrdenDetalleService)
        {
            _logger = logger;
            _OrdenDetalleService = OrdenDetalleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("OrdenDetalle.Ver")]

        public async Task<ActionResult<ApiResponse<List<OrdenDetalleDto>>>> GetOrdenDetalle()
        {
            _logger.LogInformation("📢Solicitud para obtener todos los OrdenDetalles");
            // Llama la capa de servicios para obtener el listado de orden detalles.
            var response = await _OrdenDetalleService.ObtenerTodosLosOrdenDetalleAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetOrdenDetalle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("OrdenDetalle.VerDetalle")]
        public async Task<ActionResult<ApiResponse<OrdenDetalleDto>>> GetOrdenDetalle(int id)
        {
            
            _logger.LogInformation("🔍Solicitud para obtener el OrdenDetalle con ID {OrdenDetalleId}", id);
            // Consulta al servicio por el detalle de la orden detalle solicitada.
            var response = await _OrdenDetalleService.ObtenerOrdenDetallePorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("OrdenDetalle.Crear")]
        public async Task<ActionResult<ApiResponse<OrdenDetalleDto>>> CrearOrdenDetalle([FromBody] OrdenDetalleCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo OrdenDetalle.");
            // Solicita la creación de la orden detalle en la capa de servicios
            var response = await _OrdenDetalleService.CrearOrdenDetalleAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear OrdenDetalle: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetOrdenDetalle", new { id = carrito?.OrdenDetalleId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("OrdenDetalle.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarOrdenDetalle(int id)
        {
            _logger.LogInformation("Iniciando eliminación del OrdenDetalle con ID {OrdenDetalleId}", id);
            // Solicita al servicio eliminar la orden detalle indicada.
            var response = await _OrdenDetalleService.EliminarOrdenDetalleAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("OrdenDetalle.Actualizar")]
        public async Task<ActionResult<ApiResponse<OrdenDetalleDto>>> ActualizarOrdenDetalle(int id, [FromBody] OrdenDetalleUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando orden detalle con ID {Id}.", id);
            // Solicita la actualización completa de la orden detalle.
            var response = await _OrdenDetalleService.ActualizarOrdenDetalleAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("OrdenDetalle.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<OrdenDetalleDto>>> UpdateParcialOrdenDetalle(int id, [FromBody] JsonPatchDocument<OrdenDetalleUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de OrdenDetalle con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _OrdenDetalleService.ActualizarParcialOrdenDetalleAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
