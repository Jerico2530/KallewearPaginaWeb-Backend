/*
 * Proyecto Empresarial – Controlador de Noticias
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las noticias dentro del sistema. Expone endpoints RESTful que permiten
 * operaciones de consulta, creación, actualización, eliminación y
 * exportación de registros a formatos externos.
 *
 * Funcionalidades clave:
 * - Obtener el listado completo de noticias.
 * - Consultar el detalle de una noticia específica.
 * - Crear nuevas noticias y actualizar o eliminar noticias existentes.
 * - Exportar información a un archivo Excel para fines administrativos.
 *
 * Propósito del componente:
 * Centralizar la administración del ciclo de vida de las noticias mediante
 * peticiones HTTP, delegando la lógica de negocio al servicio correspondiente.
 * El controlador es responsable de gestionar validaciones, estructurar
 * respuestas consistentes y retornar códigos de estado apropiados.
 *
 * Descripción general del código:
 * - Se inyectan el logger y el servicio de noticias.
 * - Cada endpoint está protegido mediante permisos definidos en atributos.
 * - ApiResponse se emplea como estructura común para las respuestas.
 * - El controlador maneja los códigos HTTP según el resultado de cada operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Noticia;
using BiblotecaWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticiaController : ControllerBase
    {
        private readonly ILogger<NoticiaController> _logger;
        private readonly INoticiaService _NoticiaService;

        public NoticiaController(ILogger<NoticiaController> logger, INoticiaService NoticiaService)
        {
            _logger = logger;
            _NoticiaService = NoticiaService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Noticia.Ver")]


        public async Task<ActionResult<ApiResponse<List<NoticiaDto>>>> GetNoticia()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Noticias");
            // Llama la capa de servicios para obtener el listado de noticias.
            var response = await _NoticiaService.ObtenerTodosLosNoticiaAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetNoticia")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Noticia.VerDetalle")]

        public async Task<ActionResult<ApiResponse<NoticiaDto>>> GetNoticia(int id)
        {
            _logger.LogInformation("🔍 Solicitando Noticia con ID {NoticiaId}.", id);
            // Consulta al servicio por el detalle de la noticia solicitada.
            var response = await _NoticiaService.ObtenerNoticiaPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Noticia.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _NoticiaService.ExportarExcelNoticiasAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "noticias.xlsx"
            );
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Noticia.Crear")]
        public async Task<ActionResult<ApiResponse<NoticiaDto>>> CrearNoticia([FromBody] NoticiaCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Noticia.");
            // Solicita la creación de la noticia en la capa de servicios.
            var response = await _NoticiaService.CrearNoticiaAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear Noticia: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetNoticia", new { id = carrito?.NoticiaId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Noticia.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarNoticia(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Noticia con ID {NoticiaId}", id);
            // Solicita al servicio eliminar la noticia indicada.
            var response = await _NoticiaService.EliminarNoticiaAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Noticia.Actualizar")]
        public async Task<ActionResult<ApiResponse<NoticiaDto>>> ActualizarNoticia(int id, [FromBody] NoticiaUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando noticia con ID {Id}.", id);
            // Solicita la actualización completa de la noticia.
            var response = await _NoticiaService.ActualizarNoticiaAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Noticia.ActualizarParcial")]
        public async Task<ActionResult<ApiResponse<NoticiaDto>>> UpdateParcialNoticia(int id, [FromBody] JsonPatchDocument<NoticiaUpdateDto> patchDto)
        {
            
            _logger.LogInformation("🧩 Actualización parcial de Noticia con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _NoticiaService.ActualizarParcialNoticiaAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
