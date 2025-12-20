/*
* Proyecto Empresarial – Controlador de Órdenes
* ------------------------------------------------------------
* Este componente implementa el controlador responsable de gestionar
* el ciclo de vida de las órdenes dentro del sistema. Proporciona
* endpoints RESTful para operaciones de consulta, creación, actualización,
* eliminación y exportación de datos.
*
* Funcionalidades clave:
* - Obtener todas las órdenes registradas.
* - Consultar el detalle de una orden mediante su ID.
* - Crear nuevas órdenes.
* - Actualizar órdenes (total o parcialmente).
* - Eliminar órdenes.
* - Exportar la información a un archivo Excel.
*
* Propósito del componente:
* Centralizar la gestión de solicitudes HTTP relacionadas con órdenes,
* delegando la lógica de negocio al servicio correspondiente y controlando
* los flujos de validación, respuesta y códigos de estado.
*
* Descripción general del código:
* - Se inyectan las dependencias del logger y el servicio de órdenes.
* - Cada acción está protegida por permisos mediante atributos personalizados.
* - Se utiliza ApiResponse como estructura estándar de respuesta.
* - Las respuestas incluyen códigos HTTP coherentes con cada operación.
*/

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Dto.Orden;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenController : ControllerBase
    {
        private readonly ILogger<OrdenController> _logger;
        private readonly IOrdenService _OrdenService;

        public OrdenController(ILogger<OrdenController> logger, IOrdenService OrdenService)
        {
            _logger = logger;
            _OrdenService = OrdenService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Orden.Ver")]

        public async Task<ActionResult<ApiResponse<List<OrdenDto>>>> GetOrden()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Ordens");
            // Llama la capa de servicios para obtener el listado de ordens.
            var response = await _OrdenService.ObtenerTodosLosOrdenAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetOrden")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Orden.VerDetalle")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> GetOrden(int id)
        {
            _logger.LogInformation("🔍 Solicitando Orden con ID {OrdenId}.", id);
            // Consulta al servicio por el detalle de la orden solicitada.
            var response = await _OrdenService.ObtenerOrdenPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Orden.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _OrdenService.ExportarExcelOrdenesAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "orden.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Orden.Crear")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> CrearOrden([FromBody] OrdenCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Orden.");
            // Solicita la creación de la orden en la capa de servicios.
            var response = await _OrdenService.CrearOrdenAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Orden: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetOrden", new { id = carrito?.OrdenId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Orden.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarOrden(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Orden con ID {OrdenId}", id);
            // Solicita al servicio eliminar la orden indicada.
            var response = await _OrdenService.EliminarOrdenAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Orden.Actualizar")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> ActualizarOrden(int id, [FromBody] OrdenUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando orden con ID {Id}.", id);
            // Solicita la actualización completa de la orden.
            var response = await _OrdenService.ActualizarOrdenAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Orden.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<OrdenDto>>> UpdateParcialOrden(int id, [FromBody] JsonPatchDocument<OrdenUpdateDto> patchDto)
        {
           
            _logger.LogInformation("🧩 Actualización parcial de Orden con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _OrdenService.ActualizarParcialOrdenAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
