/* 
 * Proyecto Empresarial – Controlador de Usuarios
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de la
 * gestión de usuarios dentro del sistema. Expone endpoints RESTful
 * para realizar operaciones de lectura, creación, actualización,
 * eliminación y exportación de información.
 *
 * Funcionalidades clave:
 * - Obtener todos los usuarios registrados.
 * - Consultar el usuario actual autenticado.
 * - Consultar el detalle de un usuario específico.
 * - Crear, actualizar (total o parcial) y eliminar usuarios.
 * - Exportar la lista de usuarios a un archivo Excel.
 *
 * Propósito del componente:
 * Centralizar la orquestación de las solicitudes HTTP relacionadas
 * al ciclo de vida de los usuarios, delegando la lógica de negocio
 * a los servicios correspondientes, gestionando permisos,
 * validaciones y códigos de estado.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de usuarios.
 * - Cada ruta está protegida por permisos específicos.
 * - Se utiliza ApiResponse como formato estándar de retorno.
 * - Cada endpoint entrega códigos HTTP coherentes según la operación.
 */

using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using AutoMapper;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Net;
using System.Security.Claims;

namespace ApiRopa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class UsuarioController : ControllerBase
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioService _UsuarioService;

        public UsuarioController(ILogger<UsuarioController> logger, IUsuarioService usuarioService)
        {
            _logger = logger;
            _UsuarioService = usuarioService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("Usuario.Ver")]

        public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> GetUsuario()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los Usuarios");
            // Llama la capa de servicios para obtener el listado de usuarios.
            var response = await _UsuarioService.ObtenerTodosLosUsuarioAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Usuario.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _UsuarioService.ExportarExcelUsuariosAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "usuarios.xlsx"
            );
        }

        [HttpGet("actual")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize] // 🔒 Todos necesitan un JWT válido (incluido el invitado)
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetUsuarioActual()
        {
            // Obtiene el ID de usuario desde el token JWT.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Valida que el token contenga un ID válido.
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(ResponseHelper.Fail<UsuarioDto>("Token inválido o expirado."));
            }

            _logger.LogInformation("Solicitud para obtener el usuario actual con ID {UserId}", userId);
            // Llama al servicio para obtener la información.
            var response = await _UsuarioService.ObtenerUsuarioActualAsync(userId);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }



        [HttpGet("{id:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("Usuario.VerDetalle")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetUsuario(int id)
        {
            _logger.LogInformation("🔍 Solicitando Usuario con ID {UsuarioId}.", id);
            // Consulta al servicio por el detalle de la usuario solicitada.
            var response = await _UsuarioService.ObtenerUsuarioPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Usuario.Crear")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> CrearUsuario([FromBody] UsuarioCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo Usuario.");
            // Solicita la creación de la usuario en la capa de servicios.
            var response = await _UsuarioService.CrearUsuarioAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Extrae el resultado generado para construir la ruta de retorno.
            var dto = response.Resultado;
            // Retorna el recurso creado incluyendo su endpoint de consulta
            return CreatedAtRoute("GetUsuario", new { id = dto.UsuarioId }, response);
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("Usuario.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarUsuario(int id)
        {
            _logger.LogInformation("Iniciando eliminación del Usuario con ID {UsuarioId}", id);
            // Solicita al servicio eliminar la usuario indicada.
            var response = await _UsuarioService.EliminarUsuarioAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Usuario.Actualizar")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando usuario con ID {Id}.", id);
            // Solicita la actualización completa de la usuario.
            var response = await _UsuarioService.ActualizarUsuarioAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AutorizacionPermiso("Usuario.ActualizarParcial")]

        public async Task<ActionResult<ApiResponse<UsuarioDto>>> UpdateParcialUsuario(int id, JsonPatchDocument<UsuarioUpdateDto> patchDto)
        {
            _logger.LogInformation("🧩 Actualización parcial de Usuario con ID {Id}", id);
            // Solicita una modificación parcial mediante JSON Patch.
            var response = await _UsuarioService.ActualizarParcialUsuarioAsync(id, patchDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
