using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Categoria;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de categorías dentro del catálogo de la tienda de ropa.
 *
 * Esta interfaz define los contratos necesarios para la lógica de negocio que
 * administra las categorías de productos, garantizando una estructura organizada
 * del catálogo y respuestas consistentes hacia la capa de presentación.
 *
 * Funcionalidades clave:
 * - CRUD completo de categorías (crear, consultar, actualizar y eliminar).
 * - Soporte para actualizaciones parciales mediante JsonPatch.
 * - Exportación de la información a formato Excel para uso administrativo.
 *
 * Su propósito es actuar como capa de abstracción entre controladores y repositorios,
 * asegurando que las reglas de negocio se cumplan y que todas las operaciones
 * respondan utilizando el modelo estandarizado ApiResponse.
 */
namespace ApiRopa;

public interface ICategoriaService
{
    // Obtiene todas las categorías registradas en el sistema
    Task<ApiResponse<List<CategoriaDto>>> ObtenerTodosLosCategoriasAsync();

    // Busca una categoría específica por su identificador único
    Task<ApiResponse<CategoriaDto>> ObtenerCategoriaPorIdAsync(int id);

    // Registra una nueva categoría en el catálogo
    Task<ApiResponse<CategoriaDto>> CrearCategoriaAsync(CategoriaCreateDto dto);

    // Actualiza por completo una categoría existente
    Task<ApiResponse<CategoriaDto>> ActualizarCategoriaAsync(int id, CategoriaUpdateDto updateDto);

    // Realiza modificaciones parciales sobre la categoría utilizando JsonPatch
    Task<ApiResponse<CategoriaDto>> ActualizarParcialCategoriaAsync(int id, JsonPatchDocument<CategoriaUpdateDto> patchDto);

    // Elimina una categoría de la base de datos
    Task<ApiResponse<object>> EliminarCategoriaAsync(int id);

    // Exporta todas las categorías a un archivo Excel (uso administrativo)
    Task<ApiResponse<byte[]>> ExportarExcelCategoriasAsync();
}
