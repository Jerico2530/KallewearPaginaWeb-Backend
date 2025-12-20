using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Direccion;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de direcciones de los usuarios en la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la administración de direcciones:
 * creación, consulta, actualización (completa o parcial) y eliminación, así como
 * exportación de datos a Excel para fines administrativos.
 *
 * Funcionalidades clave:
 * - CRUD completo de direcciones.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de direcciones a formato Excel.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface IDireccionService
{
    // Obtiene todas las direcciones registradas
    Task<ApiResponse<List<DireccionDto>>> ObtenerTodosLosDireccionAsync();

    // Busca una dirección específica por su identificador
    Task<ApiResponse<DireccionDto>> ObtenerDireccionPorIdAsync(int id);

    // Registra una nueva dirección para un usuario
    Task<ApiResponse<DireccionDto>> CrearDireccionAsync(DireccionCreateDto dto);

    // Actualiza completamente una dirección existente
    Task<ApiResponse<DireccionDto>> ActualizarDireccionAsync(int id, DireccionUpdateDto updateDto);

    // Realiza actualizaciones parciales sobre una dirección usando JsonPatch
    Task<ApiResponse<DireccionDto>> ActualizarParcialDireccionAsync(int id, JsonPatchDocument<DireccionUpdateDto> patchDto);

    // Elimina una dirección del sistema
    Task<ApiResponse<object>> EliminarDireccionAsync(int id);

    // Exporta todas las direcciones a un archivo Excel
    Task<ApiResponse<byte[]>> ExportarExcelDireccionesAsync();
}
