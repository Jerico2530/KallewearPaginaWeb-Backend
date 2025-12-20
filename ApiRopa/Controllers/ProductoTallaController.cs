/* 
 * Proyecto Empresarial – Controlador de Producto-Talla
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * las relaciones entre productos y tallas dentro del sistema.
 * Proporciona endpoints RESTful para operaciones de consulta,
 * creación, actualización, eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de productos con sus tallas asociadas.
 * - Consultar el detalle de una relación específica Producto-Talla.
 * - Crear nuevas asociaciones y actualizar registros existentes.
 * - Eliminar relaciones Producto-Talla del sistema.
 * - Exportar la información en formato Excel.
 *
 * Propósito del componente:
 * Centralizar la gestión de peticiones HTTP relacionadas a las
 * asociaciones entre productos y tallas, delegando la lógica de
 * negocio a la capa de servicios, garantizando consistencia en
 * respuestas y códigos de estado.
 *
 * Descripción general del código:
 * - Inyecta un servicio especializado y un logger para auditoría.
 * - Cada endpoint se asegura mediante permisos específicos.
 * - Las respuestas siguen el formato estandarizado ApiResponse.
 * - Retorna códigos HTTP adecuados según la operación realizada.
 */


using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.ProductoTalla;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoTallaController : ControllerBase
    {
        private readonly ILogger<ProductoTallaController> _logger;
        private readonly IProductoTallaService _ProductoTallaService;

        public ProductoTallaController(ILogger<ProductoTallaController> logger, IProductoTallaService ProductoTallaService)
        {
            _logger = logger;
            _ProductoTallaService = ProductoTallaService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("ProductoTalla.Ver")]

        public async Task<ActionResult<ApiResponse<List<ProductoTallaDto>>>> GetProductoTalla()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los ProductoTallas");
            // Llama la capa de servicios para obtener el listado de producto tallas.
            var response = await _ProductoTallaService.ObtenerProductoTallaConDetallesAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("{id:int}", Name = "GetProductoTalla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("ProductoTalla.VerDetalle")]

        public async Task<ActionResult<ApiResponse<ProductoTallaDto>>> GetProductoTalla(int id)
        {
            _logger.LogInformation("🔍 Solicitando ProductoTalla con ID {ProductoTallaId}.", id);
            // Consulta al servicio por el detalle de la producto talla solicitada.
            var response = await _ProductoTallaService.ObtenerProductoTallaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("ProductoTalla.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _ProductoTallaService.ExportarExcelProductoTallasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "productoTalla.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("ProductoTalla.Crear")]
        public async Task<ActionResult<ApiResponse<ProductoTallaDto>>> CrearProductoTalla([FromBody] ProductoTallaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo ProductoTalla.");
            // Solicita la creación de la producto talla en la capa de servicios.
            var response = await _ProductoTallaService.CrearProductoTallaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear ProductoTalla: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetProductoTalla", new { id = carrito?.ProductoTallaId }, response);

        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("ProductoTalla.Eliminar")]
        public async Task<ActionResult<ApiResponse<ProductoTallaDto>>> EliminaProductoTalla(int id)
        {
            _logger.LogInformation("Iniciando eliminación del ProductoTalla con ID {ProductoTallaId}", id);
            // Solicita al servicio eliminar la producto talla indicada.
            var response = await _ProductoTallaService.EliminarProductoTallaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("ProductoTalla.Actualizar")]
        public async Task<ActionResult<ApiResponse<ProductoTallaDto>>> ActualizarProductoTallaAsync(int id, [FromBody] ProductoTallaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando producto talla con ID {Id}.", id);
            // Solicita la actualización completa de la producto talla.
            var response = await _ProductoTallaService.ActualizarProductoTallaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

    }
}
