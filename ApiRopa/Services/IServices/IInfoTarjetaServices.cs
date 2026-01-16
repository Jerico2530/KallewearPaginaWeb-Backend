using ApiRopa.Models.Responses;
using BiblotecaClass.Domain.Dto.InfoTarjetas;
using BiblotecaWeb;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiRopa.Services.IServices
{
    public interface IInfoTarjetaServices 
    {
        Task<ApiResponse<List<InfoTarjetaDto>>> ObtenerTodosLosInfoTarjetaAsync();

        // Busca una categoría específica por su identificador único
        Task<ApiResponse<InfoTarjetaDto>> ObtenerInfoTarjetaPorIdAsync(int id);

        // Registra una nueva categoría en el catálogo
        Task<ApiResponse<InfoTarjetaDto>> CrearInfoTarjetaAsync(InfoTarjetaCreateDto dto);

        Task<ApiResponse<List<InfoTarjetaDto>>> ObtenerInfoTarjetasPorUsuarioAsync(int usuarioId);

        // Actualiza por completo una categoría existente
        Task<ApiResponse<InfoTarjetaDto>> ActualizarInfoTarjetaAsync(int id, InfoTarjetaUpdateDto updateDto);

        // Realiza modificaciones parciales sobre la categoría utilizando JsonPatch
        Task<ApiResponse<InfoTarjetaDto>> ActualizarParcialInfoTarjetaAsync(int id, JsonPatchDocument<InfoTarjetaUpdateDto> patchDto);

        // Elimina una categoría de la base de datos
        Task<ApiResponse<object>> EliminarInfoTarjetaAsync(int id);

        // Exporta todas las categorías a un archivo Excel (uso administrativo)
        Task<ApiResponse<byte[]>> ExportarExcelInfoTarjetasAsync();
    }
}
