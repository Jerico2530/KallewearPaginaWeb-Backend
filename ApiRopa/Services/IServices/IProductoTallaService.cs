using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.ProductoTalla;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de tallas de productos dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de tallas de productos, permitiendo operaciones completas
 * de CRUD y exportación de información.
 *
 * Funcionalidades clave:
 * - CRUD completo de tallas de productos.
 * - Obtención de todas las tallas con detalles o de una talla específica.
 * - Exportación de la información a Excel para reportes administrativos.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IProductoTallaService
{
    // Obtiene todas las tallas de productos con sus detalles asociados
    Task<ApiResponse<List<ProductoTallaDto>>> ObtenerProductoTallaConDetallesAsync();

    // Obtiene una talla específica de producto por su identificador
    Task<ApiResponse<ProductoTallaDto>> ObtenerProductoTallaPorIdAsync(int id);

    // Crea una nueva talla de producto en el sistema
    Task<ApiResponse<ProductoTallaDto>> CrearProductoTallaAsync(ProductoTallaCreateDto dto);

    // Actualiza completamente una talla de producto existente
    Task<ApiResponse<ProductoTallaDto>> ActualizarProductoTallaAsync(int id, ProductoTallaUpdateDto updateDto);

    // Elimina una talla de producto del sistema
    Task<ApiResponse<object>> EliminarProductoTallaAsync(int id);

    // Exporta todas las tallas de productos en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelProductoTallasAsync();
}

