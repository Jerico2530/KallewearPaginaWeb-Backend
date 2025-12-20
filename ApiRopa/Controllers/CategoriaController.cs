/*
 * Proyecto Empresarial – Controlador de Categorías
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las categorías registradas en el sistema, proporcionando endpoints
 * RESTful para operaciones de consulta, creación, actualización,
 * eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - Obtener todas las categorías o consultar una categoría específica.
 * - Crear, actualizar (total o parcial) y eliminar categorías.
 * - Exportar listados a un archivo Excel descargable.
 *
 * Propósito del componente:
 * Centralizar el manejo de solicitudes HTTP relacionadas con categorías,
 * delegando la lógica de negocio a los servicios correspondientes y
 * gestionando validaciones, respuestas estandarizadas y códigos HTTP.
 *
 * Descripción general del código:
 * - Inyección de dependencias del servicio de categorías y del logger.
 * - Cada endpoint se protege mediante permisos basados en atributos.
 * - Las operaciones utilizan ApiResponse como estructura consistente
 *   para manejar mensajes, resultados y códigos de estado.
 * - Los métodos retornan códigos HTTP adecuados a cada operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Categoria;
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
    public class CategoriaController : ControllerBase
    {
        private readonly ILogger<CategoriaController> _logger;
        private readonly ICategoriaService _CategoriaService;

        public CategoriaController(ILogger<CategoriaController> logger, ICategoriaService CategoriaService)
        {
            _logger = logger;
            _CategoriaService = CategoriaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Categoria.Ver")]

        public async Task<ActionResult<ApiResponse<List<CategoriaDto>>>> GetCategoria()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Categorias");
            // Llama la capa de servicios para obtener el listado de categorías.
            var response = await _CategoriaService.ObtenerTodosLosCategoriasAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Categoria.VerDetalle")]
        public async Task<ActionResult<ApiResponse<CategoriaDto>>> GetCategoria(int id)
        {

            _logger.LogInformation("🔍 Solicitando Categoria con ID {CategoriaId}.", id);
            // Consulta al servicio por el detalle de la categoría solicitada.
            var response = await _CategoriaService.ObtenerCategoriaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Categoria.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _CategoriaService.ExportarExcelCategoriasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "categoria.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Categoria.Crear")]
        public async Task<ActionResult<ApiResponse<CategoriaDto>>> CrearCategoria([FromBody] CategoriaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Categoria.");
            // Solicita la creación de la categoría en la capa de servicios.
            var response = await _CategoriaService.CrearCategoriaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Categoria: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetCategoria", new { id = carrito?.CategoriaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Categoria.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarCategoria(int id)
        {


            _logger.LogInformation("Iniciando eliminación del Categoria con ID {CategoriaId}", id);
            // Solicita al servicio eliminar la categoría indicada.
            var response = await _CategoriaService.EliminarCategoriaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Categoria.Actualizar")]
        public async Task<ActionResult<ApiResponse<CategoriaDto>>> ActualizarCategoria(int id, [FromBody] CategoriaUpdateDto updateDto)
        {

            _logger.LogInformation("🔄 Actualizando categoría con ID {Id}.", id);
            // Solicita la actualización completa de la categoría.
            var response = await _CategoriaService.ActualizarCategoriaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Categoria.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<CategoriaDto>>> UpdateParcialCategoria(int id, [FromBody] JsonPatchDocument<CategoriaUpdateDto> patchDto)
        {


           _logger.LogInformation("🧩 Actualización parcial de Categoria con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _CategoriaService.ActualizarParcialCategoriaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }
    }
}

