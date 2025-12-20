using ApiRopa.Models.Responses;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Descuento;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiRopa;

public interface IDescuentoService
{
    Task<ApiResponse<List<DescuentoDto>>> ObtenerTodosLosDescuentoAsync();
    Task<ApiResponse<List<DescuentoDto>>> ObtenerDescuentosActivosAsync();
    Task<ApiResponse<DescuentoDto>> ObtenerDescuentoPorIdAsync(int id);
    Task<ApiResponse<DescuentoDto>> CrearDescuentoAsync(DescuentoCreateDto dto);
    Task<ApiResponse<DescuentoDto>> ActualizarDescuentoAsync(int id, DescuentoUpdateDto updateDto);
    Task<ApiResponse<DescuentoDto>> ActualizarParcialDescuentoAsync(int id, JsonPatchDocument<DescuentoUpdateDto> patchDto);
    Task<ApiResponse<object>> EliminarDescuentoAsync(int id);
}
