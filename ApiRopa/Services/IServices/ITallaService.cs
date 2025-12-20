using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Talla;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de tallas de productos dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * a la administración de tallas, permitiendo operaciones CRUD completas
 * y exportación de datos para reportes.
 *
 * Funcionalidades clave:
 * - CRUD completo de tallas de productos.
 * - Obtención de todas las tallas o de una talla específica.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de tallas a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa;

public interface ITallaService
{
    // Obtiene todas las tallas registradas
    Task<ApiResponse<List<TallaDto>>> ObtenerTodosLosTallaAsync();

    // Obtiene una talla específica según su identificador
    Task<ApiResponse<TallaDto>> ObtenerTallaPorIdAsync(int id);

    // Crea una nueva talla en el sistema
    Task<ApiResponse<TallaDto>> CrearTallaAsync(TallaCreateDto dto);

    // Actualiza completamente una talla existente
    Task<ApiResponse<TallaDto>> ActualizarTallaAsync(int id, TallaUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una talla usando JsonPatch
    Task<ApiResponse<TallaDto>> ActualizarParcialTallaAsync(int id, JsonPatchDocument<TallaUpdateDto> patchDto);

    // Elimina una talla del sistema
    Task<ApiResponse<object>> EliminarTallaAsync(int id);

    // Exporta todas las tallas en formato Excel
    Task<ApiResponse<byte[]>> ExportarExcelTallasAsync();
}

