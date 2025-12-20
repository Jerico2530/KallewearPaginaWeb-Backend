/* 
 * Proyecto Empresarial – Controlador de Autenticación (Login)
 * ------------------------------------------------------------
 * Este componente implementa el controlador responsable de gestionar
 * el proceso de autenticación dentro del sistema. Expone endpoints
 * RESTful para el inicio de sesión de usuarios registrados y el
 * acceso como invitado.
 *
 * Funcionalidades clave:
 * - Autenticar usuarios mediante credenciales enviadas por el cliente.
 * - Permitir el inicio de sesión como invitado sin validación de datos.
 * - Retornar un token de autenticación y los datos mínimos necesarios
 *   para la sesión.
 *
 * Propósito del componente:
 * Centralizar el proceso de autenticación, delegando la validación
 * y generación de tokens al servicio correspondiente. Maneja los
 * códigos de estado, la consistencia de las respuestas y la exposición
 * segura de los recursos públicos de acceso.
 *
 * Descripción general del código:
 * - Se inyectan dependencias del logger y del servicio de login.
 * - Los endpoints permiten acceso anónimo, ya que son públicos.
 * - Se utiliza ApiResponse como formato estándar en todas las respuestas.
 * - Cada acción retorna el código HTTP correspondiente dependiendo
 *   del resultado de la operación.
 */

using ApiRopa.Models.Dtos;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ApiRopa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ILoginService _loginService;


        public LoginController(ILogger<LoginController> logger, ILoginService loginService )
        {
            _logger = logger;
            _loginService = loginService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResultDto>>> Login([FromBody] UsuarioLoginDto loginDto)
        {
            // Solicita la creación de la categoría en la capa de login usuario.
            var response = await _loginService.LoginAsync(loginDto);
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost("login-invitado")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<LoginResultDto>>> LoginInvitado()
        {
            // Solicita la creación de la categoría en la capa de login invitado.
            var response = await _loginService.LoginInvitadoAsync();
            // Retorna la respuesta con el código HTTP y el contenido generado por el servicio.
            return StatusCode((int)response.StatusCode, response);
        }

    }
}

