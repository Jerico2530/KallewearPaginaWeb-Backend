using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Historia;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de historias dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la administración de historias:
 * creación, consulta, actualización (completa o parcial), eliminación y exportación de datos.
 *
 * Funcionalidades clave:
 * - CRUD completo de historias.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de información a formato Excel.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */

namespace ApiRopa;

public interface IHistoriaService
{
    // Obtiene todas las historias registradas en el sistema
    Task<ApiResponse<List<HistoriaDto>>> ObtenerTodosLosHistoriaAsync();

    // Busca una historia específica por su identificador
    Task<ApiResponse<HistoriaDto>> ObtenerHistoriaPorIdAsync(int id);

    // Registra una nueva historia
    Task<ApiResponse<HistoriaDto>> CrearHistoriaAsync(HistoriaCreateDto dto);

    // Actualiza completamente una historia existente
    Task<ApiResponse<HistoriaDto>> ActualizarHistoriaAsync(int id, HistoriaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una historia usando JsonPatch
    Task<ApiResponse<HistoriaDto>> ActualizarParcialHistoriaAsync(int id, JsonPatchDocument<HistoriaUpdateDto> patchDto);

    // Elimina una historia del sistema
    Task<ApiResponse<object>> EliminarHistoriaAsync(int id);

    // Exporta todas las historias a un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelHistoriasAsync();
}

