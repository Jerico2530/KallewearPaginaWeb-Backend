using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.TipoPago;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de tipos de pago dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio relacionada
 * con la administración de métodos de pago, incluyendo operaciones CRUD completas,
 * actualizaciones parciales y exportación de datos para reportes.
 *
 * Funcionalidades clave:
 * - CRUD completo de tipos de pago.
 * - Obtención de todos los tipos de pago o de un tipo específico.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de tipos de pago a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface ITipoPagoService
{
    // Obtiene todos los tipos de pago registrados
    Task<ApiResponse<List<TipoPagoDto>>> ObtenerTodosLosTipoPagoAsync();

    // Obtiene un tipo de pago específico según su identificador
    Task<ApiResponse<TipoPagoDto>> ObtenerTipoPagoPorIdAsync(int id);

    // Crea un nuevo tipo de pago en el sistema
    Task<ApiResponse<TipoPagoDto>> CrearTipoPagoAsync(TipoPagoCreateDto dto);

    // Actualiza completamente un tipo de pago existente
    Task<ApiResponse<TipoPagoDto>> ActualizarTipoPagoAsync(int id, TipoPagoUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un tipo de pago usando JsonPatch
    Task<ApiResponse<TipoPagoDto>> ActualizarParcialTipoPagoAsync(int id, JsonPatchDocument<TipoPagoUpdateDto> patchDto);

    // Elimina un tipo de pago del sistema
    Task<ApiResponse<object>> EliminarTipoPagoAsync(int id);

    // Exporta todos los tipos de pago en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelTipoPagosAsync();
}
