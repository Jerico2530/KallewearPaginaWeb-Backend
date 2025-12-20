using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Descuento;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ApiRopa;

public class DescuentoService : IDescuentoService
{
    private readonly IDescuentoRepositorio _DescuentoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DescuentoService> _logger;
    private readonly IValidator<DescuentoCreateDto> _createValidator;
    private readonly IValidator<DescuentoUpdateDto> _updateValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;

    public DescuentoService(IDescuentoRepositorio DescuentoRepo, IMapper mapper, ILogger<DescuentoService> logger, IValidator<DescuentoCreateDto> createValidator, IValidator<DescuentoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator)
    {
        _DescuentoRepo = DescuentoRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
    }

    public async Task<ApiResponse<List<DescuentoDto>>> ObtenerTodosLosDescuentoAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Descuentos activos...");

            var descuentos = await _DescuentoRepo.ObtenerTodo();

            if (descuentos == null || !descuentos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Descuentos registrados.");
                return ResponseHelper.Fail<List<DescuentoDto>>(
                    new List<ErrorDetail> { new() { Campo = "Descuentos", Mensaje = "No se encontraron Descuentos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var descuentosDto = _mapper.Map<IEnumerable<DescuentoDto>>(descuentos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Descuentos.", descuentosDto.Count);
            return ResponseHelper.Success(descuentosDto, "Descuentos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Descuentos.");
            return ResponseHelper.FailException<List<DescuentoDto>>(ex);
        }
    }


    // Para el público: solo descuentos activos por fecha
    public async Task<ApiResponse<List<DescuentoDto>>> ObtenerDescuentosActivosAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo descuentos activos...");

            var descuentos = await _DescuentoRepo.ObtenerTodo();
            var fechaActual = DateTime.UtcNow; // Evitar problemas de zona horaria

            // Filtrar solo los descuentos activos por fecha y estado
            var descuentosActivos = descuentos
                .Where(d => d.FechaInicio <= fechaActual && d.FechaFin >= fechaActual && d.Estado)
                .ToList();

            if (descuentosActivos == null || !descuentosActivos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Descuentos registrados.");
                return ResponseHelper.Fail<List<DescuentoDto>>(
                    new List<ErrorDetail> { new() { Campo = "Descuentos", Mensaje = "No se encontraron Descuentos activos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var descuentosDto = _mapper.Map<List<DescuentoDto>>(descuentosActivos);

            _logger.LogInformation("✅ Se obtuvieron {Count} descuentos activos.", descuentosDto.Count);
            return ResponseHelper.Success(descuentosDto, "Descuentos activos obtenidos exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener descuentos activos.");
            return ResponseHelper.FailException<List<DescuentoDto>>(ex);
        }
    }




    public async Task<ApiResponse<DescuentoDto>> ObtenerDescuentoPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DescuentoDto>(validation.Errors);


            var Descuento = await _DescuentoRepo.Obtener(p => p.DescuentoId == id);

            if (Descuento == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Descuento con ID {Id}.", id);
                return ResponseHelper.Fail<DescuentoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Descuento con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<DescuentoDto>(Descuento);
            _logger.LogInformation("✅ Descuento con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Descuento encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Descuento por ID {Id}", id);
            return ResponseHelper.FailException<DescuentoDto>(ex);
        }

    }

    public async Task<ApiResponse<DescuentoDto>> CrearDescuentoAsync(DescuentoCreateDto createDto)
    {
        try
        {

            if (createDto == null)
            {
                return ResponseHelper.Fail<DescuentoDto>("Datos inválidos para crear Descuento.", "Descuento");
            }

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DescuentoDto>(validation.Errors);

            var existeDescuento = await _DescuentoRepo.Obtener(p => p.NombreDescuento.ToLower() == createDto.NombreDescuento.ToLower());
            if (existeDescuento != null)
                return ResponseHelper.Fail<DescuentoDto>("Ya existe un Descuento con ese título.", "NombreDescuento", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Descuento>(createDto);
            await _DescuentoRepo.Crear(modelo);

            var dto = _mapper.Map<DescuentoDto>(modelo);
            _logger.LogInformation("✅ Descuento '{NombreDescuento}' creado correctamente.", dto.NombreDescuento);
            return ResponseHelper.Success(dto, "Descuento creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Descuento.");
            return ResponseHelper.FailException<DescuentoDto>(ex);
        }

    }

    public async Task<ApiResponse<object>> EliminarDescuentoAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Descuento = await _DescuentoRepo.Obtener(p => p.DescuentoId == id);
            if (Descuento == null)
                return ResponseHelper.Fail<object>("Descuento no encontrado.", "Id", HttpStatusCode.NotFound);

            await _DescuentoRepo.Remover(Descuento);
            _logger.LogInformation("✅ Descuento ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Descuento eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Descuento ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<DescuentoDto>> ActualizarDescuentoAsync(int id, DescuentoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<DescuentoDto>("Datos inválidos para actualizar Descuento.", "Descuento");


            var DescuentoExistente = await _DescuentoRepo.Obtener(a => a.DescuentoId == id, tracked: true);
            if (DescuentoExistente == null)
                return ResponseHelper.Fail<DescuentoDto>("Descuento no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DescuentoDto>(validation.Errors);

            _mapper.Map(updateDto, DescuentoExistente);
            await _DescuentoRepo.ActualizarDescuento(DescuentoExistente);

            _logger.LogInformation("✅ Descuento ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<DescuentoDto>(null, "Descuento actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Descuento ID {Id}", id);
            return ResponseHelper.FailException<DescuentoDto>(ex);
        }
    }

    public async Task<ApiResponse<DescuentoDto>> ActualizarParcialDescuentoAsync(int id, JsonPatchDocument<DescuentoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<DescuentoDto>("Datos inválidos para la actualización parcial.", "Patch");

            var DescuentoExistente = await _DescuentoRepo.Obtener(a => a.DescuentoId == id, tracked: true);
            if (DescuentoExistente == null)
                return ResponseHelper.Fail<DescuentoDto>("Descuento no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<DescuentoUpdateDto>(DescuentoExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DescuentoDto>(validation.Errors);

            _mapper.Map(dto, DescuentoExistente);
            await _DescuentoRepo.ActualizarDescuento(DescuentoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Descuento ID {Id}.", id);
            return ResponseHelper.Success<DescuentoDto>(null, "Descuento actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Descuento ID {Id}", id);
            return ResponseHelper.FailException<DescuentoDto>(ex);
        }
    }
}
