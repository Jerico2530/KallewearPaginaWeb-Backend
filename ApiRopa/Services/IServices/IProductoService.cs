using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Producto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de productos dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de productos, incluyendo creación, actualización, eliminación 
 * y exportación de datos.
 *
 * Funcionalidades clave:
 * - CRUD completo de productos.
 * - Obtención de todos los productos o de un producto específico.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de información a Excel para reportes.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * garantizando respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IProductoService
{
    // Obtiene todos los productos registrados en el sistema
    Task<ApiResponse<List<ProductoDto>>> ObtenerTodosLosProductosAsync();

    // Obtiene un producto específico por su identificador
    Task<ApiResponse<ProductoDto>> ObtenerProductoPorIdAsync(int id);

    // Crea un nuevo producto en el sistema
    Task<ApiResponse<ProductoDto>> CrearProductoAsync(ProductoCreateDto dto);

    // Actualiza completamente un producto existente
    Task<ApiResponse<ProductoDto>> ActualizarProductoAsync(int id, ProductoUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un producto usando JsonPatch
    Task<ApiResponse<ProductoDto>> ActualizarParcialProductoAsync(int id, JsonPatchDocument<ProductoUpdateDto> patchDto);

    // Elimina un producto del sistema
    Task<ApiResponse<object>> EliminarProductoAsync(int id);

    // Exporta todos los productos en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelProductosAsync();
}