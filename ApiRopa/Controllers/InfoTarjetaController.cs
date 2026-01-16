using ApiRopa.Models.Responses;
using ApiRopa.Security.Attributes;
using ApiRopa.Services.IServices;
using BiblotecaClass.Domain.Dto.InfoTarjetas;
using BiblotecaWeb;

using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoTarjetaController : ControllerBase
    {
        private readonly ILogger<InfoTarjetaController> _logger;
        private readonly IInfoTarjetaServices _InfoTarjetaService;

        public InfoTarjetaController(ILogger<InfoTarjetaController> logger, IInfoTarjetaServices InfoTarjetaService)
        {
            _logger = logger;
            _InfoTarjetaService = InfoTarjetaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("InfoTarjeta.Ver")]

        public async Task<ActionResult<ApiResponse<List<InfoTarjetaDto>>>> GetInfoTarjeta()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los InfoTarjetas");
            // Llama la capa de servicios para obtener el listado de infoTarjetas.
            var response = await _InfoTarjetaService.ObtenerTodosLosInfoTarjetaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetInfoTarjeta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("InfoTarjeta.VerDetalle")]
        public async Task<ActionResult<ApiResponse<InfoTarjetaDto>>> GetInfoTarjeta(int id)
        {
            _logger.LogInformation("🔍 Solicitando InfoTarjeta con ID {InfoTarjetaId}.", id);
            // Consulta al servicio por el detalle de la infoTarjeta solicitada.
            var response = await _InfoTarjetaService.ObtenerInfoTarjetaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("InfoTarjeta.VerUsuario")]
        public async Task<ActionResult<ApiResponse<List<InfoTarjetaDto>>>> GetInfoTarjetasPorUsuario(int usuarioId)
        {
            _logger.LogInformation("📢 Solicitud de InfoTarjetas para Usuario {UsuarioId}", usuarioId);

            var response = await _InfoTarjetaService.ObtenerInfoTarjetasPorUsuarioAsync(usuarioId);

            return StatusCode((int)response.StatusCode, response);
        }


        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("InfoTarjeta.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _InfoTarjetaService.ExportarExcelInfoTarjetasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "infoTarjeta.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("InfoTarjeta.Crear")]
        public async Task<ActionResult<ApiResponse<InfoTarjetaDto>>> CrearInfoTarjeta([FromBody] InfoTarjetaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo InfoTarjeta.");
            // Solicita la creación de la infoTarjeta en la capa de servicios.
            var response = await _InfoTarjetaService.CrearInfoTarjetaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear InfoTarjeta: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetInfoTarjeta", new { id = carrito?.InfoTarjetaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("InfoTarjeta.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarInfoTarjeta(int id)
        {
            _logger.LogInformation("Iniciando eliminación del InfoTarjeta con ID {InfoTarjetaId}", id);
            // Solicita al servicio eliminar la infoTarjeta indicada.
            var response = await _InfoTarjetaService.EliminarInfoTarjetaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("InfoTarjeta.Actualizar")]
        public async Task<ActionResult<ApiResponse<InfoTarjetaDto>>> ActualizarInfoTarjeta(int id, [FromBody] InfoTarjetaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando infoTarjeta con ID {Id}.", id);
            // Solicita la actualización completa de la infoTarjeta.
            var response = await _InfoTarjetaService.ActualizarInfoTarjetaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("InfoTarjeta.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<InfoTarjetaDto>>> UpdateParcialInfoTarjeta(int id, [FromBody] JsonPatchDocument<InfoTarjetaUpdateDto> patchDto)
        {

            _logger.LogInformation("🧩 Actualización parcial de InfoTarjeta con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _InfoTarjetaService.ActualizarParcialInfoTarjetaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}

