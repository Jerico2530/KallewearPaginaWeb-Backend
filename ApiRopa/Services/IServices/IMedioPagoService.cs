using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.MedioPago;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de métodos de pago dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * al manejo de medios de pago: creación, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de medios de pago.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 * - Exportación de la lista de medios de pago en formato Excel.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IMedioPagoService
{
    // Obtiene todos los métodos de pago registrados
    Task<ApiResponse<List<MedioPagoDto>>> ObtenerTodosLosMedioPagoAsync();

    // Obtiene un método de pago específico por su identificador
    Task<ApiResponse<MedioPagoDto>> ObtenerMedioPagoPorIdAsync(int id);

    // Crea un nuevo método de pago
    Task<ApiResponse<MedioPagoDto>> CrearMedioPagoAsync(MedioPagoCreateDto dto);

    // Actualiza un método de pago existente reemplazando sus datos
    Task<ApiResponse<MedioPagoDto>> ActualizarMedioPagoAsync(int id, MedioPagoUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre un método de pago usando JsonPatch
    Task<ApiResponse<MedioPagoDto>> ActualizarParcialMedioPagoAsync(int id, JsonPatchDocument<MedioPagoUpdateDto> patchDto);

    // Elimina un método de pago de la base de datos
    Task<ApiResponse<object>> EliminarMedioPagoAsync(int id);

    // Exporta todos los métodos de pago en un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelMedioPagosAsync();
}
