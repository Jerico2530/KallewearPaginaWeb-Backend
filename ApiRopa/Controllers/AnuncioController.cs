/*
 * Proyecto Empresarial – Controlador de Anuncios
 * ------------------------------------------------------------
 * Este componente implementa el controlador encargado de gestionar
 * anuncios dentro del sistema. Expone endpoints RESTful para
 * operaciones de lectura, creación, actualización, eliminación
 * y exportación de información.
 *
 * Funcionalidades clave:
 * - Obtener todos los anuncios o versiones filtradas para administradores.
 * - Consultar el detalle de un anuncio específico.
 * - Crear, actualizar (total o parcial) y eliminar anuncios.
 * - Exportar listados de anuncios a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la orquestación de peticiones HTTP relacionadas
 * al ciclo de vida de los anuncios, delegando la lógica de negocio
 * a los servicios correspondientes y gestionando respuestas,
 * validaciones y códigos de estado.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de anuncios.
 * - Cada endpoint está protegido mediante permisos específicos.
 * - Se emplea ApiResponse como estructura estándar de respuesta.
 * - Las acciones retornan códigos HTTP adecuados para cada resultado.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using ApiRopa.Services;
using ApiRopa.Services.IServices;
using BiblotecaWeb.Domain.Dto.Anuncio;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnuncioController : ControllerBase
    {
        private readonly ILogger<AnuncioController> _logger;
        private readonly IAnuncioService _AnuncioService;

        public AnuncioController(ILogger<AnuncioController> logger, IAnuncioService AnuncioService)
        {
            _logger = logger;
            _AnuncioService = AnuncioService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Anuncio.Ver")]

        public async Task<ActionResult<ApiResponse<List<AnuncioDto>>>> GetAnuncio()
        {
            _logger.LogInformation("📢 Solicitud para obtener todos los anuncios.");
            // Llama la capa de servicios para traer el listado de anuncios.
            var response = await _AnuncioService.ObtenerTodosLosAnuncioAsync();
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpGet("admin/todos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Anuncio.VerTodos")] 
        public async Task<ActionResult<ApiResponse<List<AnuncioDto>>>> GetTodosLosAnunciosAdmin()
        {
            _logger.LogInformation("🧑‍💼 Solicitud ADMIN para obtener todos los anuncios sin filtros.");
            // Obtiene anuncios sin restricciones para uso administrativo.
            var response = await _AnuncioService.ObtenerTodosLosAnunciosAdminAsync();
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetAnuncio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Anuncio.VerDetalle")]
        public async Task<ActionResult<ApiResponse<AnuncioDto>>> GetAnuncio(int id)
        {
            _logger.LogInformation("🔍 Solicitando anuncio con ID {AnuncioId}.", id);
            // Solicita el detalle del anuncio al servicio.
            var response = await _AnuncioService.ObtenerAnuncioPorIdAsync(id);
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Anuncio.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Obtiene el archivo Excel generado por el servicio.
            var response = await _AnuncioService.ExportarExcelAnunciosAsync();

            // Devuelve error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                return StatusCode((int)response.StatusCode, response);

            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "anuncio.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Anuncio.Crear")]

        public async Task<ActionResult<ApiResponse<AnuncioDto>>> CrearAnuncio([FromBody] AnuncioCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo anuncio.");
            // Solicita la creación del anuncio a la capa de servicios.
            var response = await _AnuncioService.CrearAnuncioAsync(createDto);
            // Si la creación falla, retorna el error correspondiente.
            if (!response.IsExitoso)
                return StatusCode((int)response.StatusCode, response);
            // Devuelve el recurso recién creado con referencia a su endpoint de consulta.
            var anuncioDto = response.Resultado;
            // Retorna la respuesta estándar con código correspondiente.
            return CreatedAtRoute("GetAnuncio", new { id = anuncioDto?.AnuncioId }, response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Anuncio.Eliminar")]

        public async Task<ActionResult<ApiResponse<object>>> EliminarAnuncio(int id)
        {
            _logger.LogInformation("🗑️ Eliminando anuncio con ID {Id}.", id);
            // Pide al servicio eliminar el anuncio indicado.
            var response = await _AnuncioService.EliminarAnuncioAsync(id);
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Anuncio.Actualizar")]

        public async Task<ActionResult<ApiResponse<AnuncioDto>>> ActualizarAnuncio(int id, [FromBody] AnuncioUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando anuncio con ID {Id}.", id);
            // Solicita actualización completa del anuncio.
            var response = await _AnuncioService.ActualizarAnuncioAsync(id, updateDto);
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Anuncio.ActualizarParcial")]

        public async Task<ActionResult<ApiResponse<AnuncioDto>>> UpdateParcialAnuncio(int id, [FromBody] JsonPatchDocument<AnuncioUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial del anuncio ID {Id}.", id);
            // Solicita una actualización parcial utilizando JSON Patch.
            var response = await _AnuncioService.ActualizarParcialAnuncioAsync(id, patchDto);
            // Retorna la respuesta estándar con código correspondiente.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}

