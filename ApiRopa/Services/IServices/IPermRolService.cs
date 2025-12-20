using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.PermRol;
using BiblotecaWeb.Model.Dto;
/*
 * Servicio de gestión de permisos asignados a roles dentro del sistema de administración de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de relaciones entre permisos y roles, permitiendo mantener 
 * la seguridad y los accesos correctamente configurados.
 *
 * Funcionalidades clave:
 * - CRUD completo de relaciones Permiso-Rol.
 * - Obtención de permisos asignados a roles con sus detalles.
 * - Exportación de datos a Excel para auditorías o reportes.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IPermRolService
{
    // Obtiene todos los permisos asignados a roles, incluyendo detalles de cada relación
    Task<ApiResponse<List<PermRolDto>>> ObtenerPermRolConDetallesAsync();

    // Crea una nueva relación Permiso-Rol
    Task<ApiResponse<PermRolDto>> CrearPermRolAsync(PermRolCreateDto dto);

    // Obtiene una relación Permiso-Rol específica por su identificador
    Task<ApiResponse<PermRolDto>> ObtenerPermRolPorIdAsync(int id);

    // Actualiza una relación Permiso-Rol existente reemplazando su información
    Task<ApiResponse<PermRolDto>> ActualizarPermRolAsync(int id, PermRolUpdateDto updateDto);

    // Elimina una relación Permiso-Rol del sistema
    Task<ApiResponse<object>> EliminarPermRolAsync(int id);

    // Exporta todas las relaciones Permiso-Rol en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelPermRolesAsync();
}
