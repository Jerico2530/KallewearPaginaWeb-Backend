/* 
 * Proyecto Empresarial – Controlador de Productos
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * los productos dentro del sistema. Expone endpoints RESTful para
 * operaciones de consulta, creación, actualización, eliminación
 * y exportación de datos.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de productos y consultar detalles.
 * - Crear nuevos productos y actualizar registros existentes.
 * - Eliminar productos del sistema.
 * - Exportar datos de productos a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la administración de solicitudes HTTP relacionadas
 * al ciclo de vida de los productos, delegando la lógica de negocio
 * a la capa de servicios y garantizando respuestas consistentes.
 *
 * Descripción general del código:
 * - Inyecta el servicio de productos y el logger especializado.
 * - Cada endpoint está protegido mediante permisos definidos.
 * - Utiliza ApiResponse como estructura estándar de retorno.
 * - Las acciones devuelven códigos HTTP adecuados según el resultado.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Producto;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly ILogger<ProductoController> _logger;
        private readonly IProductoService _ProductoService;

        public ProductoController(ILogger<ProductoController> logger, IProductoService productoService)
        {
            _logger = logger;
            _ProductoService = productoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Producto.Ver")]
        public async Task<ActionResult<ApiResponse<List<ProductoDto>>>> GetProducto()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Productos");
            // Llama la capa de servicios para obtener el listado de productos.
            var response = await _ProductoService.ObtenerTodosLosProductosAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetProducto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Producto.VerDetalle")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> GetProducto(int id)
        {
            _logger.LogInformation("🔍 Solicitando Producto con ID {ProductoId}.", id);
            // Consulta al servicio por el detalle de la producto solicitada.
            var response = await _ProductoService.ObtenerProductoPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Producto.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _ProductoService.ExportarExcelProductosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "productos.xlsx"
            );
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Producto.Crear")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> CrearProducto([FromBody] ProductoCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Producto.");
            // Solicita la creación de la producto en la capa de servicios.
            var response = await _ProductoService.CrearProductoAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Producto: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetProducto", new { id = carrito?.ProductoId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Producto.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarProducto(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Producto con ID {ProductoId}", id);
            // Solicita al servicio eliminar la producto indicada.
            var response = await _ProductoService.EliminarProductoAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Producto.Actualizar")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> ActualizarProducto(int id, [FromBody] ProductoUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando producto con ID {Id}.", id);
            // Solicita la actualización completa de la producto.
            var response = await _ProductoService.ActualizarProductoAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Producto.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> UpdateParcialProducto(int id, [FromBody] JsonPatchDocument<ProductoUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Producto con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _ProductoService.ActualizarParcialProductoAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
