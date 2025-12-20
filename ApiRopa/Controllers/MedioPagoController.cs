/*
 * Proyecto Empresarial – Plataforma de Gestión de Catálogo, Usuarios y Transacciones
 * ---------------------------------------------------------------------------------
 * Este proyecto implementa una solución empresarial completa para la administración
 * de recursos comerciales, usuarios, seguridad, pagos y flujo transaccional. Expone
 * una API RESTful estructurada, modular y organizada en componentes independientes
 * pero integrados, orientados a proveer operaciones consistentes, seguras y escalables.
 *
 * Funcionalidades clave:
 * - Gestión integral de entidades del catálogo (anuncios, medios de pago, medio pago, etc.).
 * - Autenticación de usuarios mediante credenciales, tokens JWT y acceso como invitado.
 * - Control de permisos y autorización por roles mediante atributos personalizados.
 * - Operaciones CRUD completas y exportación de datos a formatos externos como Excel.
 * - Estandarización de respuestas mediante ApiResponse para mantener consistencia.
 *
 * Propósito del componente:
 * Este proyecto centraliza toda la lógica requerida para administrar los recursos 
 * principales del sistema, manejar autenticación y autorización, procesar operaciones 
 * de negocio y exponer endpoints seguros para uso administrativo y público. Su diseño 
 * busca garantizar orden, trazabilidad, extensibilidad y mantenimiento a largo plazo.
 *
 * Descripción general del código:
 * - Se organiza la solución en controladores, servicios y repositorios, aplicando 
 *   separación clara de responsabilidades.
 * - Los controladores exponen endpoints REST, aplican permisos y gestionan códigos HTTP.
 * - Los servicios encapsulan la lógica de negocio y utilizan DTOs para transportar datos.
 * - Los repositorios interactúan con la capa de datos respetando principios SOLID.
 * - Se emplea un logger para registrar solicitudes, resultados y posibles incidencias.
 * - Los componentes comparten un modelo estándar de respuesta (ApiResponse) para 
 *   asegurar uniformidad en la comunicación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using ApiRopa.Services.IServices;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.MedioPago;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedioPagoController : ControllerBase
    {
        private readonly ILogger<MedioPagoController> _logger;
        private readonly IMedioPagoService _MedioPagoService;

        public MedioPagoController(ILogger<MedioPagoController> logger, IMedioPagoService MedioPagoService)
        {
            _logger = logger;
            _MedioPagoService = MedioPagoService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("MedioPago.Ver")]

        public async Task<ActionResult<ApiResponse<List<MedioPago>>>> GetMedioPagol()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los MedioPagos");
            // Llama la capa de servicios para obtener el listado de medio pago.
            var response = await _MedioPagoService.ObtenerTodosLosMedioPagoAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("{id:int}", Name = "GetMedioPago")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("MedioPago.VerDetalle")]

        public async Task<ActionResult<ApiResponse<MedioPagoDto>>> GetMedioPago(int id)
        {
            _logger.LogInformation("🔍 Solicitando MedioPago con ID {MedioPagoId}.", id);
            // Consulta al servicio por el detalle de la medio pago solicitada.
            var response = await _MedioPagoService.ObtenerMedioPagoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("MedioPago.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _MedioPagoService.ExportarExcelMedioPagosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "medioPago.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("MedioPago.Crear")]

        public async Task<ActionResult<ApiResponse<MedioPagoDto>>> CrearMedioPagol([FromBody] MedioPagoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo MedioPago.");
            // Solicita la creación de la medio pago en la capa de servicios.
            var response = await _MedioPagoService.CrearMedioPagoAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear MedioPago: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetMedioPago", new { id = carrito?.MedioPagoId }, response);

        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("MedioPago.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarMedioPago(int id)
        {
            _logger.LogInformation("Iniciando eliminación del MedioPago con ID {MedioPagoId}", id);
            // Solicita al servicio eliminar la medio pago indicada.
            var response = await _MedioPagoService.EliminarMedioPagoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("MedioPago.Actualizar")]
        public async Task<ActionResult<ApiResponse<MedioPagoDto>>> ActualizarMedioPago(int id, [FromBody] MedioPagoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando medio pago con ID {Id}.", id);
            // Solicita la actualización completa de la medio pago.
            var response = await _MedioPagoService.ActualizarMedioPagoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }




        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("MedioPago.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<MedioPagoDto>>> UpdateParcialMedioPago(int id, JsonPatchDocument<MedioPagoUpdateDto> patchDto)
        {

            _logger.LogInformation("🧩 Actualización parcial de MedioPago con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _MedioPagoService.ActualizarParcialMedioPagoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}

