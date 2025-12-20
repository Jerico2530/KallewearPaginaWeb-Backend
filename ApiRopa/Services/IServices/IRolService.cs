using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Rol;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de roles dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de roles de usuarios, permitiendo operaciones de CRUD completas
 * y exportación de datos para reportes.
 *
 * Funcionalidades clave:
 * - CRUD completo de roles de usuario.
 * - Obtención de roles individuales o listados completos.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de roles a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IRolService
{
    // Obtiene todos los roles registrados en el sistema
    Task<ApiResponse<List<RolDto>>> ObtenerTodosLosRolAsync();

    // Obtiene un rol específico según su identificador
    Task<ApiResponse<RolDto>> ObtenerRolPorIdAsync(int id);

    // Crea un nuevo rol en el sistema
    Task<ApiResponse<RolDto>> CrearRolAsync(RolCreateDto dto);

    // Actualiza completamente un rol existente
    Task<ApiResponse<RolDto>> ActualizarRolAsync(int id, RolUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un rol usando JsonPatch
    Task<ApiResponse<RolDto>> ActualizarParcialRolAsync(int id, JsonPatchDocument<RolUpdateDto> patchDto);

    // Elimina un rol del sistema
    Task<ApiResponse<object>> EliminarRolAsync(int id);

    // Exporta todos los roles en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelRolesAsync();
}
