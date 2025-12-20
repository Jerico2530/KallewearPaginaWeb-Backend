using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Moneda;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de monedas dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de monedas: creación, consulta, actualización y eliminación.
 *
 * Funcionalidades clave:
 * - CRUD completo de monedas.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 * - Exportación de la lista de monedas en formato Excel.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IMonedaService
{
    // Obtiene todas las monedas registradas en el sistema
    Task<ApiResponse<List<MonedaDto>>> ObtenerTodosLosMonedaAsync();

    // Obtiene una moneda específica por su identificador
    Task<ApiResponse<MonedaDto>> ObtenerMonedaPorIdAsync(int id);

    // Crea una nueva moneda en el sistema
    Task<ApiResponse<MonedaDto>> CrearMonedaAsync(MonedaCreateDto dto);

    // Actualiza una moneda existente reemplazando su información
    Task<ApiResponse<MonedaDto>> ActualizarMonedaAsync(int id, MonedaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una moneda usando JsonPatch
    Task<ApiResponse<MonedaDto>> ActualizarParcialMonedaAsync(int id, JsonPatchDocument<MonedaUpdateDto> patchDto);

    // Elimina una moneda del sistema
    Task<ApiResponse<object>> EliminarMonedaAsync(int id);

    // Exporta todas las monedas en un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelMonedasAsync();
}

