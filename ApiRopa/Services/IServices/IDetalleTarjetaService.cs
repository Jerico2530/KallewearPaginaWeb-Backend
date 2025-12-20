using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de los detalles de tarjetas asociadas a los usuarios.
 *
 * Esta interfaz define los contratos esenciales para la administración de la información
 * de tarjetas: creación, consulta, actualización (completa o parcial) y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de detalles de tarjetas.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de información a formato Excel.
 *
 * Actúa como capa de abstracción entre los controladores y la persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IDetalleTarjetaService
{
    // Obtiene todos los detalles de tarjetas registrados
    Task<ApiResponse<List<DetalleTarjetaDto>>> ObtenerTodosLosDetalleTarjetaAsync();

    // Busca un detalle de tarjeta específico por su identificador
    Task<ApiResponse<DetalleTarjetaDto>> ObtenerDetalleTarjetaPorIdAsync(int id);

    // Registra un nuevo detalle de tarjeta en el sistema
    Task<ApiResponse<DetalleTarjetaDto>> CrearDetalleTarjetaAsync(DetalleTarjetaCreateDto dto);

    // Actualiza completamente un detalle de tarjeta existente
    Task<ApiResponse<DetalleTarjetaDto>> ActualizarDetalleTarjetaAsync(int id, DetalleTarjetaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un detalle de tarjeta usando JsonPatch
    Task<ApiResponse<DetalleTarjetaDto>> ActualizarParcialDetalleTarjetaAsync(int id, JsonPatchDocument<DetalleTarjetaUpdateDto> patchDto);

    // Elimina un detalle de tarjeta del sistema
    Task<ApiResponse<object>> EliminarDetalleTarjetaAsync(int id);

    // Exporta todos los detalles de tarjetas a un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelDetalleTarjetasAsync();
}
