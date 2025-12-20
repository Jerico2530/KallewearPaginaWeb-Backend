using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Noticia;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de noticias dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de noticias: creación, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de noticias.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 * - Exportación de la lista de noticias en formato Excel.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface INoticiaService
{
    // Obtiene todas las noticias registradas en el sistema
    Task<ApiResponse<List<NoticiaDto>>> ObtenerTodosLosNoticiaAsync();

    // Obtiene una noticia específica por su identificador
    Task<ApiResponse<NoticiaDto>> ObtenerNoticiaPorIdAsync(int id);

    // Crea una nueva noticia en el sistema
    Task<ApiResponse<NoticiaDto>> CrearNoticiaAsync(NoticiaCreateDto dto);

    // Actualiza una noticia existente reemplazando su información
    Task<ApiResponse<NoticiaDto>> ActualizarNoticiaAsync(int id, NoticiaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una noticia usando JsonPatch
    Task<ApiResponse<NoticiaDto>> ActualizarParcialNoticiaAsync(int id, JsonPatchDocument<NoticiaUpdateDto> patchDto);

    // Elimina una noticia del sistema
    Task<ApiResponse<object>> EliminarNoticiaAsync(int id);

    // Exporta todas las noticias en un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelNoticiasAsync();
}

