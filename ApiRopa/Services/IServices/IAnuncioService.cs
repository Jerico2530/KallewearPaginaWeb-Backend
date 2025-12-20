using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Dto.Anuncio;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiRopa.Services.IServices
{
    /// <summary>
    /// Define el contrato del servicio de Anuncios dentro de la capa de aplicación.
    /// Proporciona las operaciones CRUD completas, obtención filtrada para administración
    /// y funcionalidades adicionales como eliminación y exportación en formato Excel.
    /// 
    /// Este servicio abstrae la lógica de negocio y facilita la escalabilidad,
    /// al permitir que el controlador interactúe con la capa de dominio de forma desacoplada.
    /// </summary>
    public interface IAnuncioService
    {
        /// <summary>
        /// Obtiene todos los anuncios visibles públicamente.
        /// </summary>
        Task<ApiResponse<List<AnuncioDto>>> ObtenerTodosLosAnuncioAsync();

        /// <summary>
        /// Obtiene todos los anuncios con datos extendidos para administración interna.
        /// </summary>
        Task<ApiResponse<List<AnuncioDto>>> ObtenerTodosLosAnunciosAdminAsync();

        /// <summary>
        /// Devuelve un anuncio específico por su identificador único.
        /// </summary>
        Task<ApiResponse<AnuncioDto>> ObtenerAnuncioPorIdAsync(int id);

        /// <summary>
        /// Crea un nuevo anuncio utilizando un DTO diseñado para creación.
        /// </summary>
        Task<ApiResponse<AnuncioDto>> CrearAnuncioAsync(AnuncioCreateDto dto);

        /// <summary>
        /// Actualiza completamente un anuncio existente mediante un DTO destinado a edición.
        /// </summary>
        Task<ApiResponse<AnuncioDto>> ActualizarAnuncioAsync(int id, AnuncioUpdateDto updateDto);

        /// <summary>
        /// Realiza una actualización parcial usando operaciones JSON Patch.
        /// Permite modificar campos específicos sin reemplazar el recurso completo.
        /// </summary>
        Task<ApiResponse<AnuncioDto>> ActualizarParcialAnuncioAsync(int id, JsonPatchDocument<AnuncioUpdateDto> patchDto);

        /// <summary>
        /// Elimina un anuncio por su ID y retorna un resultado genérico.
        /// </summary>
        Task<ApiResponse<object>> EliminarAnuncioAsync(int id);

        /// <summary>
        /// Exporta los anuncios a un archivo Excel y retorna el contenido en bytes.
        /// </summary>
        Task<ApiResponse<byte[]>> ExportarExcelAnunciosAsync();

    }
}
