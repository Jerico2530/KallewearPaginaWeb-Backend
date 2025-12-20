using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de usuarios dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la administración de usuarios,
 * incluyendo creación, consulta, actualización, eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - CRUD completo de usuarios.
 * - Obtención de usuario específico o del usuario actualmente autenticado.
 * - Aplicación de actualizaciones parciales mediante JsonPatch.
 * - Exportación de información de usuarios a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IUsuarioService
{
    // Obtiene todos los usuarios registrados en el sistema
    Task<ApiResponse<List<UsuarioDto>>> ObtenerTodosLosUsuarioAsync();

    // Obtiene un usuario específico por su identificador
    Task<ApiResponse<UsuarioDto>> ObtenerUsuarioPorIdAsync(int id);

    // Obtiene la información del usuario actualmente autenticado
    Task<ApiResponse<UsuarioDto>> ObtenerUsuarioActualAsync(int userId);

    // Crea un nuevo usuario en el sistema
    Task<ApiResponse<UsuarioDto>> CrearUsuarioAsync(UsuarioCreateDto dto);

    // Actualiza completamente la información de un usuario existente
    Task<ApiResponse<UsuarioDto>> ActualizarUsuarioAsync(int id, UsuarioUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un usuario usando JsonPatch
    Task<ApiResponse<UsuarioDto>> ActualizarParcialUsuarioAsync(int id, JsonPatchDocument<UsuarioUpdateDto> patchDto);

    // Elimina un usuario del sistema
    Task<ApiResponse<object>> EliminarUsuarioAsync(int id);

    // Exporta la información de todos los usuarios a un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelUsuariosAsync();


}
