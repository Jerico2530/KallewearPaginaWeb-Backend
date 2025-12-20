using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Permiso;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de permisos dentro del sistema de administración de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * al manejo de permisos de usuarios: creación, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de permisos.
 * - Exportación de permisos a Excel.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IPermisoService
{
    // Obtiene todos los permisos registrados en el sistema
    Task<ApiResponse<List<PermisoDto>>> ObtenerTodosLosPermisoAsync();

    // Obtiene un permiso específico por su identificador
    Task<ApiResponse<PermisoDto>> ObtenerPermisoPorIdAsync(int id);

    // Registra un nuevo permiso en el sistema
    Task<ApiResponse<PermisoDto>> CrearPermisoAsync(PermisoCreateDto dto);

    // Actualiza un permiso existente reemplazando su información
    Task<ApiResponse<PermisoDto>> ActualizarPermisoAsync(int id, PermisoUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un permiso usando JsonPatch
    Task<ApiResponse<PermisoDto>> ActualizarParcialPermisoAsync(int id, JsonPatchDocument<PermisoUpdateDto> patchDto);

    // Elimina un permiso del sistema
    Task<ApiResponse<object>> EliminarPermisoAsync(int id);

    // Exporta todos los permisos en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelPermisosAsync();
}
