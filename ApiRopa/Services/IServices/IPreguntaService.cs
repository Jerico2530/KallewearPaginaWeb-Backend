using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Pregunta;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de preguntas dentro del sistema de atención o interacción con usuarios.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de preguntas frecuentes o consultas, asegurando que 
 * el flujo de información se gestione de manera consistente y estructurada.
 *
 * Funcionalidades clave:
 * - CRUD completo de preguntas.
 * - Obtención de preguntas por identificador.
 * - Actualizaciones parciales usando JsonPatch.
 * - Exportación de datos a Excel para reportes o auditorías.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * estandarizando las respuestas mediante ApiResponse.
 */
namespace ApiRopa;

public interface IPreguntaService
{
    // Obtiene todas las preguntas registradas en el sistema
    Task<ApiResponse<List<PreguntaDto>>> ObtenerTodosLosPreguntaAsync();

    // Obtiene una pregunta específica por su identificador
    Task<ApiResponse<PreguntaDto>> ObtenerPreguntaPorIdAsync(int id);

    // Crea una nueva pregunta en el sistema
    Task<ApiResponse<PreguntaDto>> CrearPreguntaAsync(PreguntaCreateDto dto);

    // Actualiza completamente una pregunta existente
    Task<ApiResponse<PreguntaDto>> ActualizarPreguntaAsync(int id, PreguntaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una pregunta usando JsonPatch
    Task<ApiResponse<PreguntaDto>> ActualizarParcialPreguntaAsync(int id, JsonPatchDocument<PreguntaUpdateDto> patchDto);

    // Elimina una pregunta del sistema
    Task<ApiResponse<object>> EliminarPreguntaAsync(int id);

    // Exporta todas las preguntas en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelPreguntasAsync();
}
