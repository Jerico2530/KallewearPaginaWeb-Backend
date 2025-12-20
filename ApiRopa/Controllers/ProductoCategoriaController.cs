/* 
 * Proyecto Empresarial – Controlador de ProductoCategoria
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * las relaciones entre productos y producto categorias dentro del sistema.
 * Expone endpoints RESTful para la obtención, creación, actualización
 * y eliminación de asociaciones, además de exportación en formato Excel.
 *
 * Funcionalidades clave:
 * - Consultar todas las relaciones Producto–Categoría con sus detalles.
 * - Obtener información de una relación específica.
 * - Registrar, actualizar y eliminar relaciones entre productos y producto categorias.
 * - Exportar el listado completo a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la orquestación de solicitudes HTTP relacionadas
 * a la administración de ProductoCategoria, delegando la lógica
 * de negocio a los servicios correspondientes y gestionando 
 * validaciones, códigos de estado y respuestas estándar.
 *
 * Descripción general del código:
 * - Se inyectan dependencias de logging y del servicio de ProductoCategoria.
 * - Cada endpoint utiliza permisos específicos mediante AutorizacionPermiso.
 * - Las respuestas se normalizan con ApiResponse para mantener consistencia.
 * - Se retornan códigos HTTP coherentes con el resultado de cada operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoCategoriaController : ControllerBase
    {
        private readonly ILogger<ProductoCategoriaController> _logger;
        private readonly IProductoCategoriaService _ProductoCategoriaService;

        public ProductoCategoriaController(ILogger<ProductoCategoriaController> logger, IProductoCategoriaService Producto_ProductoCategoriaService)
        {
            _logger = logger;
            _ProductoCategoriaService = Producto_ProductoCategoriaService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("ProductoCategoria.Ver")]
        public async Task<ActionResult<ApiResponse<List<ProductoCategoriaDto>>>> GetProductoCategoria()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los ProductoCategorias");
            // Llama la capa de servicios para obtener el listado de producto categorias.
            var response = await _ProductoCategoriaService.ObtenerProductoCategoriasConDetallesAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("{id:int}", Name = "GetProductoCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("ProductoCategoria.VerDetalle")]

        public async Task<ActionResult<ApiResponse<ProductoCategoriaDto>>> GetProductoCategoria(int id)
        {
            _logger.LogInformation("🔍 Solicitando ProductoCategoria con ID {ProductoCategoriaId}.", id);
            // Consulta al servicio por el detalle de la producto categoria solicitada.
            var response = await _ProductoCategoriaService.ObtenerProductoCategoriaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("ProductoCategoria.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _ProductoCategoriaService.ExportarExcelProductoCategoriasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "productoCategorias.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("ProductoCategoria.Crear")]

        public async Task<ActionResult<ApiResponse<ProductoCategoriaDto>>> CrearProductoCategoria([FromBody] ProductoCategoriaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo ProductoCategoria.");
            // Solicita la creación de la producto categoria en la capa de servicios.
            var response = await _ProductoCategoriaService.CrearProductoCategoriaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Extrae el resultado generado para construir la ruta de retorno.
            var productocategoria = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetProductoCategoria", new { id = productocategoria?.ProductoCategoriaId }, response);

        }


        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("ProductoCategoria.Eliminar")]

        public async Task<ActionResult<ApiResponse<object>>> EliminaProductoCategoria(int id)
        {

            _logger.LogInformation("Iniciando eliminación del ProductoCategoria con ID {ProductoCategoriaId}", id);
            // Solicita al servicio eliminar la producto categoria indicada.
            var response = await _ProductoCategoriaService.EliminarProductoCategoriaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("ProductoCategoria.Actualizar")]

        public async Task<ActionResult<ApiResponse<ProductoCategoriaDto>>> ActualizarProductoCategoriaAsync(int id, [FromBody] ProductoCategoriaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando producto categoria con ID {Id}.", id);
            // Solicita la actualización completa de la producto categoria.
            var response = await _ProductoCategoriaService.ActualizarProductoCategoriaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }




    }
}

