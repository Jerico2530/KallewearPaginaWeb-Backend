using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Genero;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de géneros dentro del catálogo de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la administración de géneros:
 * creación, consulta, actualización (completa o parcial), eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - CRUD completo de géneros.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de información a formato Excel.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IGeneroService
{
    // Obtiene todos los géneros registrados en el sistema
    Task<ApiResponse<List<GeneroDto>>> ObtenerTodosLosGeneroAsync();

    // Busca un género específico por su identificador
    Task<ApiResponse<GeneroDto>> ObtenerGeneroPorIdAsync(int id);

    // Registra un nuevo género
    Task<ApiResponse<GeneroDto>> CrearGeneroAsync(GeneroCreateDto dto);

    // Actualiza completamente un género existente
    Task<ApiResponse<GeneroDto>> ActualizarGeneroAsync(int id, GeneroUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un género usando JsonPatch
    Task<ApiResponse<GeneroDto>> ActualizarParcialGeneroAsync(int id, JsonPatchDocument<GeneroUpdateDto> patchDto);

    // Elimina un género del sistema
    Task<ApiResponse<object>> EliminarGeneroAsync(int id);

    // Exporta todos los géneros a un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelGenerosAsync();
}

