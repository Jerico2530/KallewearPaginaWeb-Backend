/* 
 * Proyecto Empresarial – Controlador de Pagos
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * los pagos dentro del sistema. Expone endpoints RESTful para 
 * operaciones de lectura, creación, actualización, eliminación 
 * y exportación de datos.
 *
 * Funcionalidades clave:
 * - Obtener todos los pagos registrados en el sistema.
 * - Consultar el detalle de un pago específico.
 * - Registrar nuevos pagos.
 * - Actualizar información de pagos (completa o parcial).
 * - Eliminar registros de pagos.
 * - Exportar listados de pagos a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la orquestación de solicitudes HTTP relacionadas 
 * al ciclo de vida de los pagos, delegando la lógica de negocio 
 * a la capa de servicios y gestionando códigos de estado, respuestas 
 * y validaciones.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de pagos.
 * - Cada endpoint utiliza permisos específicos definidos en el sistema.
 * - Se utiliza ApiResponse como contrato estándar de respuesta.
 * - Las operaciones retornan códigos HTTP según el resultado obtenido.
 */

using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Domain.Dto.Pago;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly ILogger<PagoController> _logger;
        private readonly IPagoService _PagoService;

        public PagoController(ILogger<PagoController> logger, IPagoService PagoService)
        {
            _logger = logger;
            _PagoService = PagoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Pago.Ver")]

        public async Task<ActionResult<ApiResponse<List<PagoDto>>>> GetPago()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Pagos");
            // Llama la capa de servicios para obtener el listado de pagos.
            var response = await _PagoService.ObtenerTodosLosPagoAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetPago")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Pago.VerDetalle")]
        public async Task<ActionResult<ApiResponse<PagoDto>>> GetPago(int id)
        {
            _logger.LogInformation("🔍 Solicitando Pago con ID {PagoId}.", id);
            // Consulta al servicio por el detalle de la pago solicitada.
            var response = await _PagoService.ObtenerPagoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Pago.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _PagoService.ExportarExcelPagosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pago.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Pago.Crear")]
        public async Task<ActionResult<ApiResponse<PagoDto>>> CrearPago([FromBody] PagoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Pago.");

            try
            {
                // 1️⃣ Obtener usuarioId desde el token correctamente
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (usuarioIdClaim == null)
                {
                    _logger.LogWarning("❌ Token inválido o expirado.");
                    return Unauthorized(ResponseHelper.Fail<PagoDto>("Token inválido o expirado."));
                }

                int usuarioId = int.Parse(usuarioIdClaim.Value);

                // 2️⃣ Llamar al servicio de pagos
                var response = await _PagoService.CrearPagoAsync(createDto, usuarioId);

                // 3️⃣ Manejo de errores
                if (!response.IsExitoso)
                {
                    _logger.LogWarning("❌ Error al crear Pago: {@Response}", response);
                    return StatusCode((int)response.StatusCode, response);
                }

                // 4️⃣ Pago creado correctamente
                var pago = response.Resultado;
                return CreatedAtRoute("GetPago", new { id = pago?.PagoId }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error inesperado al crear Pago");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ResponseHelper.FailException<PagoDto>(ex));
            }
        }




        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Pago.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarPago(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Pago con ID {PagoId}", id);
            // Solicita al servicio eliminar la pago indicada.
            var response = await _PagoService.EliminarPagoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Pago.Actualizar")]
        public async Task<ActionResult<ApiResponse<PagoDto>>> ActualizarPago(int id, [FromBody] PagoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando pago con ID {Id}.", id);
            // Solicita la actualización completa de la pago.
            var response = await _PagoService.ActualizarPagoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Pago.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<PagoDto>>> UpdateParcialPago(int id, [FromBody] JsonPatchDocument<PagoUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Pago con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _PagoService.ActualizarParcialPagoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Pago.Ver")]
        public async Task<ActionResult<ApiResponse<List<PagoDto>>>> GetPagosPorUsuario(int usuarioId)
        {
            _logger.LogInformation("🔍 Solicitud para obtener pagos del Usuario ID {UsuarioId}",usuarioId);
            // 🔹 Llamada al service (lógica de negocio centralizada)
            var response = await _PagoService.ObtenerPagosPorUsuarioAsync(usuarioId);
            // 🔹 Retorna exactamente el contrato del service
            return StatusCode((int)response.StatusCode, response);
        }

    }
}