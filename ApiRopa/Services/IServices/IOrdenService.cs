using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Orden;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de órdenes dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * al manejo de órdenes: creación, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de órdenes.
 * - Exportación de órdenes a Excel.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IOrdenService
{
    // Obtiene todas las órdenes registradas en el sistema
    Task<ApiResponse<List<OrdenDto>>> ObtenerTodosLosOrdenAsync();

    // Obtiene una orden específica por su identificador
    Task<ApiResponse<OrdenDto>> ObtenerOrdenPorIdAsync(int id);

    // Crea una nueva orden en el sistema
    Task<ApiResponse<OrdenDto>> CrearOrdenAsync(OrdenCreateDto dto);

    // Actualiza una orden existente reemplazando su información
    Task<ApiResponse<OrdenDto>> ActualizarOrdenAsync(int id, OrdenUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una orden usando JsonPatch
    Task<ApiResponse<OrdenDto>> ActualizarParcialOrdenAsync(int id, JsonPatchDocument<OrdenUpdateDto> patchDto);

    // Elimina una orden del sistema
    Task<ApiResponse<object>> EliminarOrdenAsync(int id);

    // Exporta todas las órdenes en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelOrdenesAsync();
}
