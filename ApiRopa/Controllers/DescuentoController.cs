using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Descuento;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DescuentoController : ControllerBase
    {
        private readonly ILogger<DescuentoController> _logger;
        private readonly IDescuentoService _DescuentoService;

        public DescuentoController(ILogger<DescuentoController> logger, IDescuentoService DescuentoService)
        {
            _logger = logger;
            _DescuentoService = DescuentoService;
        }

        [HttpGet("activos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<DescuentoDto>>>> GetDescuentoActivo()
        {
            _logger.LogInformation("Solicitud para obtener descuentos activos (público)");
            var response = await _DescuentoService.ObtenerDescuentosActivosAsync();
            return StatusCode((int)response.StatusCode, response);
        }
        

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Descuento.Ver")]
        public async Task<ActionResult<ApiResponse<List<DescuentoDto>>>> GetDescuentoAdmin()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Descuentos");
            var response = await _DescuentoService.ObtenerTodosLosDescuentoAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetDescuento")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Descuento.VerDetalle")]
        public async Task<ActionResult<ApiResponse<AnuncioDto>>> GetDescuento(int id)
        {
            _logger.LogInformation("🔍 Solicitando Descuento con ID {DescuentoId}.", id);
            var response = await _DescuentoService.ObtenerDescuentoPorIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Descuento.Crear")]
        public async Task<ActionResult<ApiResponse<DescuentoDto>>> CrearDescuento([FromBody] DescuentoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Descuento.");

            var response = await _DescuentoService.CrearDescuentoAsync(createDto);

            if (!response.IsExitoso)
                return StatusCode((int)response.StatusCode, response);

            var carrito = response.Resultado ;
            return CreatedAtRoute("GetDescuento", new { id = carrito?.DescuentoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Descuento.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarDescuento(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Descuento con ID {DescuentoId}", id);
            var response = await _DescuentoService.EliminarDescuentoAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Descuento.Actualizar")]

        public async Task<ActionResult<ApiResponse<DescuentoDto>>> ActualizarDescuento(int id, [FromBody] DescuentoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando categoría con ID {Id}.", id);
            var response = await _DescuentoService.ActualizarDescuentoAsync(id, updateDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Descuento.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<DescuentoDto>>> UpdateParcialDescuento(int id, [FromBody] JsonPatchDocument<DescuentoUpdateDto> patchDto)
        {
           
            _logger.LogInformation("🧩 Actualización parcial de Descuento con ID {Id}", id);
            var response = await _DescuentoService.ActualizarParcialDescuentoAsync(id, patchDto);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
