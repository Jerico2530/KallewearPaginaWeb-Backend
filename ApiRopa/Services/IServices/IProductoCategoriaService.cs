using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Model.Dto;
/*
 * Servicio de gestión de categorías de productos dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de categorías de productos, asegurando consistencia y 
 * estandarización en las operaciones sobre datos de categoría.
 *
 * Funcionalidades clave:
 * - CRUD completo de categorías de productos.
 * - Obtención de categorías con detalles asociados.
 * - Exportación de información a Excel para reportes.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * garantizando respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IProductoCategoriaService
{
    // Obtiene todas las categorías de productos junto con sus detalles asociados
    Task<ApiResponse<List<ProductoCategoriaDto>>> ObtenerProductoCategoriasConDetallesAsync();

    // Obtiene una categoría de producto específica por su identificador
    Task<ApiResponse<ProductoCategoriaDto>> ObtenerProductoCategoriaPorIdAsync(int id);

    // Crea una nueva categoría de producto
    Task<ApiResponse<ProductoCategoriaDto>> CrearProductoCategoriaAsync(ProductoCategoriaCreateDto dto);

    // Actualiza completamente una categoría de producto existente
    Task<ApiResponse<ProductoCategoriaDto>> ActualizarProductoCategoriaAsync(int id, ProductoCategoriaUpdateDto updateDto);

    // Elimina una categoría de producto del sistema
    Task<ApiResponse<object>> EliminarProductoCategoriaAsync(int id);

    // Exporta todas las categorías de productos en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelProductoCategoriasAsync();
}
