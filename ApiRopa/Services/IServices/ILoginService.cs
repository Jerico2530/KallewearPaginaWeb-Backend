using ApiRopa.Models.Dtos;
using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de autenticación de usuarios para la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para el proceso de inicio de sesión:
 * autenticación de usuarios registrados y acceso como invitado.
 *
 * Funcionalidades clave:
 * - Login de usuario con credenciales.
 * - Login de invitado sin necesidad de registro.
 *
 * Actúa como capa de abstracción entre controladores y la lógica de autenticación,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface ILoginService
{

    // Autentica un usuario registrado con sus credenciales y devuelve información de sesión
    Task<ApiResponse<LoginResultDto>> LoginAsync(UsuarioLoginDto loginDto);

    // Permite el acceso temporal como invitado, sin necesidad de registro
    Task<ApiResponse<LoginResultDto>> LoginInvitadoAsync();

}
