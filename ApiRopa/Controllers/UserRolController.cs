/*
 * Proyecto Empresarial – Controlador de UserRol
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * las relaciones entre usuarios y roles dentro del sistema. Expone
 * endpoints RESTful para consultar, crear, actualizar y eliminar 
 * asociaciones entre usuarios y sus roles, además de permitir la
 * exportación de datos en formato Excel.
 *
 * Funcionalidades clave:
 * - Obtener todos los registros UserRol o consultar uno específico.
 * - Crear nuevas asignaciones de roles a usuarios.
 * - Actualizar o eliminar asociaciones existentes.
 * - Exportar el listado completo de UserRol a un archivo Excel.
 *
 * Propósito del componente:
 * Administrar y exponer las rutas HTTP relacionadas con la gestión de
 * roles asignados a usuarios, delegando la lógica de negocio al servicio
 * correspondiente y garantizando respuestas consistentes, autorizaciones
 * adecuadas y códigos de estado apropiados.
 *
 * Descripción general del código:
 * - Inyecta un logger y el servicio encargado de manejar los UserRol.
 * - Cada endpoint está protegido con permisos específicos.
 * - Utiliza ApiResponse como estructura estándar para devolver resultados.
 * - Cada acción retorna códigos HTTP acordes al resultado de la operación.
 */

using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Security;
using ApiRopa.Security.Attributes;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.UserRol;
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

    public class UserRolController : ControllerBase
    {
        private readonly ILogger<UserRolController> _logger;
        private readonly IUserRolService _UserRolService;

        public UserRolController(ILogger<UserRolController> logger, IUserRolService userRolService)
        {
            _logger = logger;
            _UserRolService = userRolService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AutorizacionPermiso("UserRol.Ver")]
        public async Task<ActionResult<ApiResponse<List<UserRolDto>>>> GetUserRol()
        {
            _logger.LogInformation(" 📢 Solicitud para obtener todos los UserRols");
            // Llama la capa de servicios para obtener el listado de userRols
            var response = await _UserRolService.ObtenerUserRolesConDetallesAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id:int}", Name = "GetUserRol")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("UserRol.VerDetalle")]

        public async Task<ActionResult<ApiResponse<UserRolDto>>> GetUserRol(int id)
        {
            _logger.LogInformation("🔍 Solicitando UserRol con ID {UserRolId}.", id);
            // Consulta al servicio por el detalle de la userRol solicitada.
            var response = await _UserRolService.ObtenerUserRolPorIdAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpGet("exportar-excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AutorizacionPermiso("UserRol.DescargarExcel")]
        public async Task<IActionResult> ExportarExcel()
        {
            // Solicita al servicio generar el archivo Excel.
            var response = await _UserRolService.ExportarExcelUserRolesAsync();
            // Retorna error si la exportación no fue exitosa.
            if (!response.IsExitoso)
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            // Retorna el archivo Excel en formato descargable.
            return File(
                response.Resultado,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "userRol.xlsx"
            );
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("UserRol.Crear")]
        public async Task<ActionResult<ApiResponse<UserRolDto>>> CrearUserRol([FromBody] UserRolCreateDto createDto)
        {
            _logger.LogInformation("📝 Creando nuevo UserRol.");
            // Solicita la creación de la userRol en la capa de servicios.
            var response = await _UserRolService.CrearUserRolAsync(createDto);
            // Si ocurre un error en la creación, retorna el código correspondiente.
            if (!response.IsExitoso)
            {
                _logger.LogWarning("Error al crear UserRol: {@Response}", response);
                // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
                return StatusCode((int)response.StatusCode, response);
            }
            // Extrae el resultado generado para construir la ruta de retorno.
            var carrito = response.Resultado ;
            // Retorna el recurso creado incluyendo su endpoint de consulta.
            return CreatedAtRoute("GetUserRol", new { id = carrito?.UserRolId }, response);
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AutorizacionPermiso("UserRol.Eliminar")]
        public async Task<ActionResult<ApiResponse<object>>> EliminarUserRol(int id)
        {
            _logger.LogInformation("Iniciando eliminación del UserRol con ID {UserRolId}", id);
            // Solicita al servicio eliminar la userRol indicada.
            var response = await _UserRolService.EliminarUserRolAsync(id);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);

        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  
        [AutorizacionPermiso("UserRol.Actualizar")]
        public async Task<ActionResult<ApiResponse<UserRolDto>>> ActualizarUserRol(int id, [FromBody] UserRolUpdateDto updateDto)
        {
            _logger.LogInformation("🔄 Actualizando userRol con ID {Id}.", id);
            // Solicita la actualización completa de la userRol.
            var response = await _UserRolService.ActualizarUserRolAsync(id, updateDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }


    }
}


