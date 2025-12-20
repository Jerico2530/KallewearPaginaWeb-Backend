/*
 * Proyecto Empresarial – Controlador de Detalles de Tarjeta
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * los detalles de tarjetas dentro del sistema. Expone endpoints
 * RESTful para operaciones de consulta, creación, actualización total
 * o parcial, eliminación y exportación de datos a Excel.
 *
 * Funcionalidades clave:
 * - Obtener todos los detalles de tarjetas disponibles.
 * - Consultar información específica por identificador.
 * - Crear nuevos registros de detalle de tarjeta.
 * - Actualizar registros existentes mediante PUT o JSON Patch.
 * - Eliminar registros y exportar la información en formato Excel.
 *
 * Propósito del componente:
 * Centralizar las operaciones HTTP relacionadas al ciclo de vida
 * de los detalles de tarjeta, delegando la lógica de negocio al
 * servicio correspondiente y estandarizando las respuestas del API.
 *
 * Descripción general del código:
 * - Se inyectan el logger y el servicio encargado del manejo de datos.
 * - Cada operación está protegida mediante permisos declarados.
 * - El controlador retorna respuestas normalizadas con ApiResponse.
 * - Los códigos de estado se generan según el resultado de la operación.
 */
using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleTarjetaController : ControllerBase
    {
        private readonly ILogger<DetalleTarjetaController> _logger;
        private readonly IDetalleTarjetaService _DetalleTarjetalService;

        public DetalleTarjetaController(ILogger<DetalleTarjetaController> logger, IDetalleTarjetaService DetalleTarjetaService)
        {
            _logger = logger;
            _DetalleTarjetalService = DetalleTarjetaService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("DetalleTarjeta.Ver")]
        
        public async Task<ActionResult<ApiResponse<List<DetalleTarjetaDto>>>> GetDetalleTarjetal()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los DetalleTarjetas");
            // Llama la capa de servicios para obtener el listado de categorías.
            var response = await _DetalleTarjetalService.ObtenerTodosLosDetalleTarjetaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("{id:int}", Name = "GetDetalleTarjeta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("DetalleTarjeta.VerDetalle")] 

        public async Task<ActionResult<ApiResponse<DetalleTarjetaDto>>> GetDetalleTarjeta(int id)
        {
            _logger.LogInformation("🔍 Solicitando DetalleTarjeta con ID {DetalleTarjetaId}.", id);
            // Consulta al servicio por el detalle de la detalle tarjeta solicitada.
            var response = await _DetalleTarjetalService.ObtenerDetalleTarjetaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("DetalleTarjeta.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _DetalleTarjetalService.ExportarExcelDetalleTarjetasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "detalleTarjeta.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("DetalleTarjeta.Crear")] 

        public async Task<ActionResult<ApiResponse<DetalleTarjetaDto>>> CrearDetalleTarjetal([FromBody] DetalleTarjetaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo DetalleTarjeta.");
            // Solicita la creación de la detalle tarjeta en la capa de servicios.
            var response = await _DetalleTarjetalService.CrearDetalleTarjetaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear DetalleTarjeta: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetDetalleTarjeta", new { id = carrito?.DetalleTarjetaId }, response);

        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("DetalleTarjeta.Eliminar")] 
        public async Task<ActionResult<ApiResponse<object>>> EliminarDetalleTarjeta(int id)
        {
            _logger.LogInformation("Iniciando eliminación del DetalleTarjeta con ID {DetalleTarjetaId}", id);
            // Solicita al servicio eliminar la categoría indicada.
            var response = await _DetalleTarjetalService.EliminarDetalleTarjetaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("DetalleTarjeta.Actualizar")] 
        public async Task<ActionResult<ApiResponse<DetalleTarjetaDto>>> ActualizarDetalleTarjeta(int id, [FromBody] DetalleTarjetaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando categoría con ID {Id}.", id);
            // Solicita la actualización completa de la categoría.
            var response = await _DetalleTarjetalService.ActualizarDetalleTarjetaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("DetalleTarjeta.ActualizarParcial")]
   

       public async Task<ActionResult<ApiResponse<DetalleTarjetaDto>>> UpdateParcialDetalleTarjeta(int id, JsonPatchDocument<DetalleTarjetaUpdateDto> patchDto)
        {
           
            _logger.LogInformation("🧩 Actualización parcial de DetalleTarjeta con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch
            var response = await _DetalleTarjetalService.ActualizarParcialDetalleTarjetaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
