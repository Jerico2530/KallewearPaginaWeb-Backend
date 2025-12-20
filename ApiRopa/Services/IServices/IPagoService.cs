using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Pago;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de pagos dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * al manejo de pagos: registro, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de pagos.
 * - Exportación de pagos a Excel.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IPagoService
{
    // Obtiene todos los pagos registrados en el sistema
    Task<ApiResponse<List<PagoDto>>> ObtenerTodosLosPagoAsync();

    // Obtiene un pago específico por su identificador
    Task<ApiResponse<PagoDto>> ObtenerPagoPorIdAsync(int id);

    // Registra un nuevo pago en el sistema
    Task<ApiResponse<PagoDto>> CrearPagoAsync(PagoCreateDto dto);

    // Actualiza un pago existente reemplazando su información
    Task<ApiResponse<PagoDto>> ActualizarPagoAsync(int id, PagoUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un pago usando JsonPatch
    Task<ApiResponse<PagoDto>> ActualizarParcialPagoAsync(int id, JsonPatchDocument<PagoUpdateDto> patchDto);

    // Elimina un pago del sistema
    Task<ApiResponse<object>> EliminarPagoAsync(int id);

    // Exporta todos los pagos en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelPagosAsync();
}
