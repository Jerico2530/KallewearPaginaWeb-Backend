using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.UserRol;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de la relación entre usuarios y roles dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la administración de roles asignados a usuarios,
 * incluyendo creación, consulta, actualización, eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - CRUD completo de relaciones usuario-rol.
 * - Obtención de relaciones completas o por identificador.
 * - Exportación de relaciones a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IUserRolService
{
    // Obtiene todas las relaciones usuario-rol con detalles
    Task<ApiResponse<List<UserRolDto>>> ObtenerUserRolesConDetallesAsync();

    // Obtiene una relación específica según su identificador
    Task<ApiResponse<UserRolDto>> ObtenerUserRolPorIdAsync(int id);

    // Crea una nueva relación entre usuario y rol
    Task<ApiResponse<UserRolDto>> CrearUserRolAsync(UserRolCreateDto dto);

    // Elimina una relación usuario-rol existente
    Task<ApiResponse<object>> EliminarUserRolAsync(int id);

    // Actualiza completamente una relación usuario-rol
    Task<ApiResponse<UserRolDto>> ActualizarUserRolAsync(int id, UserRolUpdateDto updateDto);

    // Exporta todas las relaciones usuario-rol en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelUserRolesAsync();


}
