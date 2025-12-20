/*
 * Proyecto Empresarial – Controlador de TipoPago
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las operaciones relacionadas con los tipos de pago dentro del sistema.
 * Expone endpoints RESTful para consultar, crear, actualizar, eliminar
 * y exportar información en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener la lista completa de tipos de pago o un detalle específico.
 * - Crear, actualizar (total o parcial) y eliminar tipos de pago.
 * - Exportar listados de tipos de pago a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la orquestación de solicitudes HTTP vinculadas al ciclo
 * de vida de los tipos de pago, delegando las reglas de negocio en la
 * capa de servicios y gestionando validaciones, respuestas y códigos
 * de estado estandarizados.
 *
 * Descripción general del código:
 * - Inyecta dependencias del logger y del servicio de tipos de pago.
 * - Cada acción usa permisos específicos como mecanismo de autorización.
 * - Utiliza ApiResponse como estructura consistente de retorno.
 * - Retorna códigos HTTP adecuados para cada operación ejecutada.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Dto.TipoPago;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoPagoController : ControllerBase
    {
        private readonly ILogger<TipoPagoController> _logger;
        private readonly ITipoPagoService _TipoPagoService;

        public TipoPagoController(ILogger<TipoPagoController> logger, ITipoPagoService TipoPagoService)
        {
            _logger = logger;
            _TipoPagoService = TipoPagoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("TipoPago.Ver")]
        public async Task<ActionResult<ApiResponse<List<TipoPagoDto>>>> GetTipoPago()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los TipoPagos");
            // Llama la capa de servicios para obtener el listado de tipo pagos.
            var response = await _TipoPagoService.ObtenerTodosLosTipoPagoAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetTipoPago")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("TipoPago.VerDetalle")]
        public async Task<ActionResult<ApiResponse<TipoPagoDto>>> GetTipoPago(int id)
        {
            _logger.LogInformation("🔍 Solicitando TipoPago con ID {TipoPagoId}.", id);
            // Consulta al servicio por el detalle de la tipo pago solicitada.
            var response = await _TipoPagoService.ObtenerTipoPagoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("TipoPago.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _TipoPagoService.ExportarExcelTipoPagosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "tipoPago.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("TipoPago.Crear")]
        public async Task<ActionResult<ApiResponse<TipoPagoDto>>> CrearTipoPago([FromBody] TipoPagoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo TipoPago.");
            // Solicita la creación de la tipo pago en la capa de servicios.
            var response = await _TipoPagoService.CrearTipoPagoAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear TipoPago: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetTipoPago", new { id = carrito?.TipoPagoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("TipoPago.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarTipoPago(int id)
        {
            _logger.LogInformation("Iniciando eliminación del TipoPago con ID {TipoPagoId}", id);
            // Solicita al servicio eliminar la tipo pago indicada.
            var response = await _TipoPagoService.EliminarTipoPagoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("TipoPago.Actualizar")]
        public async Task<ActionResult<ApiResponse<TipoPagoDto>>> ActualizarTipoPago(int id, [FromBody] TipoPagoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando tipo pago con ID {Id}.", id);
            // Solicita la actualización completa de la tipo pago.
            var response = await _TipoPagoService.ActualizarTipoPagoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("TipoPago.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<TipoPagoDto>>> UpdateParcialTipoPago(int id, [FromBody] JsonPatchDocument<TipoPagoUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de TipoPago con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _TipoPagoService.ActualizarParcialTipoPagoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
