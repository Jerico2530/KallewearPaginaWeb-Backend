/*
 * Proyecto Empresarial – Controlador de Carrito de Compra
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las operaciones del carrito de compra dentro del sistema. Proporciona
 * endpoints RESTful que permiten consultar, crear, modificar, vaciar
 * y confirmar compras asociadas al carrito de un usuario.
 *
 * Funcionalidades clave:
 * - Obtener carritos o consultar carritos asociados a un usuario.
 * - Registrar nuevos ítems dentro del carrito.
 * - Actualizar información del carrito de forma total o parcial.
 * - Eliminar un carrito o vaciar el carrito de un usuario.
 * - Calcular el subtotal del carrito.
 * - Confirmar el proceso de compra.
 *
 * Propósito del componente:
 * Centralizar las operaciones relacionadas con la administración del
 * carrito de compra, delegando la lógica de negocio al servicio 
 * correspondiente y asegurando el uso de permisos, validaciones y 
 * códigos de estado consistentes en toda la API.
 *
 * Descripción general del código:
 * - Se inyectan el logger y el servicio de carrito de compra.
 * - Todos los endpoints están protegidos mediante atributos de permisos.
 * - Las respuestas se estructuran en base al modelo ApiResponse.
 * - Cada acción interactúa con el servicio y retorna el código HTTP 
 *   adecuado según el resultado de la operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoCompraController : ControllerBase
    {
        private readonly ILogger<CarritoCompraController> _logger;
        private readonly ICarritoCompraService _CarritoCompraService;

        public CarritoCompraController(ILogger<CarritoCompraController> logger, ICarritoCompraService CarritoCompraService)
        {
            _logger = logger;
            _CarritoCompraService = CarritoCompraService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("CarritoCompra.Ver")]

        public async Task<ActionResult<ApiResponse<List<CarritoCompraDto>>>> GetCarritoCompra()
        {
            _logger.LogInformation("Solicitud para obtener todos los CarritoCompras");
            // Solicita al servicio el listado completo de carritos.
            var response = await _CarritoCompraService.ObtenerTodosLosCarritoCompraAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("usuario/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("CarritoCompra.UsuarioVer")]

        public async Task<ActionResult<ApiResponse<object>>> ObtenerTodosLosCarritoCompraUsuarioAsync(int usuarioId)
        {
            _logger.LogInformation("Solicitud para obtener todos los CarritoCompras del usuario {UsuarioId}", usuarioId);
            // Obtiene los carritos asociados a un usuario específico.
            var response = await _CarritoCompraService.ObtenerTodosLosCarritoCompraUsuarioAsync(usuarioId);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetCarritoCompra")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("CarritoCompra.VerDetalle")]
        public async Task<ActionResult<ApiResponse<CarritoCompraDto>>> GetCarritoCompra(int id)
        {
            _logger.LogInformation("🔍 Obtener CarritoCompra con ID {Id}", id);
            // Solicita el detalle del carrito según el ID proporcionado.
            var response = await _CarritoCompraService.ObtenerCarritoCompraPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        // ✅ Nuevo endpoint: vaciar carrito por usuario
        [HttpDelete("vaciar/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("CarritoCompra.VaciarPorUsuario")]
        public async Task<ActionResult<ApiResponse<object>>> VaciarCarrito(int usuarioId)
        {
            _logger.LogInformation("🧹 Vaciando carrito del usuario {UsuarioId}", usuarioId);
            // Solicita el vaciado completo del carrito asociado al usuario.
            var response = await _CarritoCompraService.VaciarCarritoAsync(usuarioId);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("CarritoCompra.Crear")]
        public async Task<ActionResult<ApiResponse<CarritoCompraDto>>> CrearCarritoCompra([FromBody] CarritoCompraCreateDto createDto)
        {
            _logger.LogInformation("🛍️ Solicitud para crear un CarritoCompra");
            // Solicita al servicio registrar un nuevo carrito.
            var response = await _CarritoCompraService.CrearCarritoCompraAsync(createDto);

            // Retorna el error si el proceso no fue exitoso.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear CarritoCompra: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetCarritoCompra", new { id = carrito?.CarritoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("CarritoCompra.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarCarritoCompra(int id)
        {
            _logger.LogInformation("🗑️ Eliminando CarritoCompra con ID {Id}", id);
            // Solicita al servicio eliminar el carrito asociado al ID indicado.
            var response = await _CarritoCompraService.EliminarCarritoCompraAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("CarritoCompra.Actualizar")]
        public async Task<ActionResult<ApiResponse<object>>> ActualizarCarritoCompra(int id, [FromBody] CarritoCompraUpdateDto updateDto)
        {
            _logger.LogInformation("✏️ Actualizando CarritoCompra con ID {Id}", id);
            // Solicita la actualización completa de los datos del carrito.
            var response = await _CarritoCompraService.ActualizarCarritoCompraAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("CarritoCompra.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<CarritoCompraDto>>> UpdateParcialCarritoCompra(int id, [FromBody] JsonPatchDocument<CarritoCompraUpdateDto> patchDto)
        {
         
            _logger.LogInformation("🧩 Actualización parcial de CarritoCompra con ID {Id}", id);
            // Solicita la modificación parcial del carrito utilizando JSON Patch.
            var response = await _CarritoCompraService.ActualizarParcialCarritoCompraAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("total/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("CarritoCompra.VerSubTotal")]
        public async Task<ActionResult<ApiResponse<CarritoCompraDto>>> ObtenerTotal(int usuarioId)
        {
            _logger.LogInformation("💰 Obteniendo total del carrito del usuario {UsuarioId}", usuarioId);
            // Solicita el cálculo del subtotal del carrito.
            var response = await _CarritoCompraService.ObtenerTotalCarritoAsync(usuarioId);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        // ✅ Nuevo endpoint: confirmar compra del carrito de un usuario
        [HttpPost("confirmar/{usuarioId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("CarritoCompra.ConfirmarCompra")]
        public async Task<ActionResult<ApiResponse<object>>> ConfirmarCompra(int usuarioId)
        {
            _logger.LogInformation("💳 Confirmando compra del carrito para el usuario {UsuarioId}", usuarioId);
            // Solicita la confirmación de la compra del carrito.
            var response = await _CarritoCompraService.ConfirmarCompraCarritoAsync(usuarioId);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


    }
}

