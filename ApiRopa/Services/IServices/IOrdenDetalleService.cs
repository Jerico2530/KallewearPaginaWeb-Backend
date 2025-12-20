using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.OrdenDetalle;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de los detalles de las órdenes dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de los ítems de cada orden: creación, consulta, actualización
 * y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de los detalles de las órdenes.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IOrdenDetalleService
{
    // Obtiene todos los detalles de órdenes registrados en el sistema
    Task<ApiResponse<List<OrdenDetalleDto>>> ObtenerTodosLosOrdenDetalleAsync();

    // Obtiene un detalle de orden específico por su identificador
    Task<ApiResponse<OrdenDetalleDto>> ObtenerOrdenDetallePorIdAsync(int id);

    // Crea un nuevo detalle de orden en el sistema
    Task<ApiResponse<OrdenDetalleDto>> CrearOrdenDetalleAsync(OrdenDetalleCreateDto dto);

    // Actualiza un detalle de orden existente reemplazando su información
    Task<ApiResponse<OrdenDetalleDto>> ActualizarOrdenDetalleAsync(int id, OrdenDetalleUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un detalle de orden usando JsonPatch
    Task<ApiResponse<OrdenDetalleDto>> ActualizarParcialOrdenDetalleAsync(int id, JsonPatchDocument<OrdenDetalleUpdateDto> patchDto);

    // Elimina un detalle de orden del sistema
    Task<ApiResponse<object>> EliminarOrdenDetalleAsync(int id);
}
