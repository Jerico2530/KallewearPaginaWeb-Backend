using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Sucursal;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de sucursales dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de sucursales, permitiendo operaciones CRUD completas
 * y exportación de datos para reportes.
 *
 * Funcionalidades clave:
 * - CRUD completo de sucursales.
 * - Obtención de sucursales individuales o listados completos.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de sucursales a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface ISucursalService
{
    // Obtiene todas las sucursales registradas
    Task<ApiResponse<List<SucursalDto>>> ObtenerTodosLosSucursalAsync();

    // Obtiene una sucursal específica según su identificador
    Task<ApiResponse<SucursalDto>> ObtenerSucursalPorIdAsync(int id);

    // Crea una nueva sucursal en el sistema
    Task<ApiResponse<SucursalDto>> CrearSucursalAsync(SucursalCreateDto dto);

    // Actualiza completamente una sucursal existente
    Task<ApiResponse<SucursalDto>> ActualizarSucursalAsync(int id, SucursalUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una sucursal usando JsonPatch
    Task<ApiResponse<SucursalDto>> ActualizarParcialSucursalAsync(int id, JsonPatchDocument<SucursalUpdateDto> patchDto);

    // Elimina una sucursal del sistema
    Task<ApiResponse<object>> EliminarSucursalAsync(int id);

    // Exporta todas las sucursales en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelSucursalesAsync();
}
