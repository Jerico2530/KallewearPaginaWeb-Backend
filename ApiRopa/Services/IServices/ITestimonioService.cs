using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Testimonio;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
/*
 * Servicio de gestión de testimonios dentro del sistema de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio relacionada
 * con la administración de testimonios de clientes, incluyendo operaciones CRUD completas,
 * actualizaciones parciales y exportación de datos para reportes.
 *
 * Funcionalidades clave:
 * - CRUD completo de testimonios.
 * - Obtención de todos los testimonios o de un testimonio específico.
 * - Actualizaciones parciales mediante JsonPatch.
 * - Exportación de testimonios a Excel para fines administrativos.
 *
 * Actúa como capa de abstracción entre los controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas mediante ApiResponse.
 */
namespace ApiRopa.Services.IServices
{
    public interface ITestimonioService
    {
        // Obtiene todos los testimonios registrados
        Task<ApiResponse<List<TestimonioDto>>> ObtenerTodosLosTestimonioAsync();

        // Obtiene un testimonio específico según su identificador
        Task<ApiResponse<TestimonioDto>> ObtenerTestimonioPorIdAsync(int id);

        // Crea un nuevo testimonio en el sistema
        Task<ApiResponse<TestimonioDto>> CrearTestimonioAsync(TestimonioCreateDto dto);

        // Actualiza completamente un testimonio existente
        Task<ApiResponse<TestimonioDto>> ActualizarTestimonioAsync(int id, TestimonioUpdateDto updateDto);

        // Realiza actualizaciones parciales sobre un testimonio usando JsonPatch
        Task<ApiResponse<TestimonioDto>> ActualizarParcialTestimonioAsync(int id, JsonPatchDocument<TestimonioUpdateDto> patchDto);

        // Elimina un testimonio del sistema
        Task<ApiResponse<object>> EliminarTestimonioAsync(int id);

        // Exporta todos los testimonios en formato Excel
        Task<ApiResponse<byte[]>> ExportarExcelTestimoniosAsync();
    }
}
