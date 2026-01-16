using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositories.Interfaces;
using ApiRopa.Services.IServices;
using AutoMapper;
using BiblotecaClass.Domain.Dto.InfoTarjetas;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ApiRopa;

public class InfoTarjetaServices : IInfoTarjetaServices
{
    private readonly IInfoTarjetaRepositorio _InfoTarjetaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<InfoTarjetaServices> _logger;
    private readonly IValidator<InfoTarjetaCreateDto> _createValidator;
    private readonly IValidator<InfoTarjetaUpdateDto> _updateValidator;
    private readonly IValidator<InfoTarjetaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public InfoTarjetaServices(IInfoTarjetaRepositorio InfoTarjetaRepo, IMapper mapper, ILogger<InfoTarjetaServices> logger, IValidator<InfoTarjetaCreateDto> createValidator, IValidator<InfoTarjetaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<InfoTarjetaUpdateDto> patchValidator,
AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _InfoTarjetaRepo = InfoTarjetaRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _patchValidator = patchValidator;
        _context = context;
        _excelGenericoService = excelGenericoService;


    }

    public async Task<ApiResponse<List<InfoTarjetaDto>>> ObtenerTodosLosInfoTarjetaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los InfoTarjetas activos...");

            var InfoTarjetas = await _InfoTarjetaRepo.ObtenerInfoTarjetasConDetalles();

            if (InfoTarjetas == null || !InfoTarjetas.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron InfoTarjetas registrados.");
                return ResponseHelper.Fail<List<InfoTarjetaDto>>(
                    new List<ErrorDetail> { new() { Campo = "InfoTarjetas", Mensaje = "No se encontraron InfoTarjetas registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var InfoTarjetasDto = _mapper.Map<IEnumerable<InfoTarjetaDto>>(InfoTarjetas).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} InfoTarjetas.", InfoTarjetasDto.Count);
            return ResponseHelper.Success(InfoTarjetasDto, "InfoTarjetas obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener InfoTarjetas.");
            return ResponseHelper.FailException<List<InfoTarjetaDto>>(ex);
        }
    }

    public async Task<ApiResponse<List<InfoTarjetaDto>>> ObtenerInfoTarjetasPorUsuarioAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo InfoTarjetas del usuario {UsuarioId}", usuarioId);

            var infoTarjetas = await _InfoTarjetaRepo.ObtenerInfoTarjetasPorUsuarioAsync(usuarioId);

            if (infoTarjetas == null || !infoTarjetas.Any())
            {
                return ResponseHelper.Fail<List<InfoTarjetaDto>>(
                    new List<ErrorDetail>
                    {
                    new()
                    {
                        Campo = "UsuarioId",
                        Mensaje = "El usuario no tiene InfoTarjetas registradas."
                    }
                    },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<List<InfoTarjetaDto>>(infoTarjetas);

            return ResponseHelper.Success(dto, "InfoTarjetas obtenidas correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener InfoTarjetas del usuario {UsuarioId}", usuarioId);
            return ResponseHelper.FailException<List<InfoTarjetaDto>>(ex);
        }
    }


    public async Task<ApiResponse<InfoTarjetaDto>> ObtenerInfoTarjetaPorIdAsync(int id)
    {
        try
        {
            // Validación de ID
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<InfoTarjetaDto>(validation.Errors);

            var InfoTarjeta = await _InfoTarjetaRepo.Obtener(a => a.InfoTarjetaId == id);
            if (InfoTarjeta == null)
            {
                _logger.LogWarning("⚠️ No se encontró el InfoTarjeta con ID {Id}.", id);
                return ResponseHelper.Fail<InfoTarjetaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el InfoTarjeta con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<InfoTarjetaDto>(InfoTarjeta);
            _logger.LogInformation("✅ InfoTarjeta con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "InfoTarjeta encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener InfoTarjeta por ID {Id}", id);
            return ResponseHelper.FailException<InfoTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelInfoTarjetasAsync()
    {
        try
        {
            var historias = await _context.InfomaTarjetas.ToListAsync();
            var historiasDto = _mapper.Map<List<InfoTarjetaDto>>(historias);

            // Generación de Excel mediante servicio auxiliar
            var bytes = await _excelGenericoService.ExportarExcel(
                historiasDto, "Reporte de InfoTarjetas", "InfoTarjetas"
            );

            if (bytes == null || bytes.Length == 0)
                return ResponseHelper.Fail<byte[]>(
                    "No se generó ningún archivo Excel.",
                    campo: null,
                    code: HttpStatusCode.NotFound
                );

            return ResponseHelper.Success(bytes, "Excel generado correctamente.");
        }
        catch (Exception ex)
        {
            return ResponseHelper.FailException<byte[]>(ex);
        }
    }


    public async Task<ApiResponse<InfoTarjetaDto>> CrearInfoTarjetaAsync(InfoTarjetaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<InfoTarjetaDto>("Datos inválidos para crear InfoTarjeta.", "InfoTarjeta");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<InfoTarjetaDto>(validation.Errors);

            var modelo = _mapper.Map<InfoTarjetas>(createDto);
            await _InfoTarjetaRepo.Crear(modelo);

            var dto = _mapper.Map<InfoTarjetaDto>(modelo);
            return ResponseHelper.Success(dto, "InfoTarjeta creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear InfoTarjeta.");
            return ResponseHelper.FailException<InfoTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarInfoTarjetaAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var InfoTarjeta = await _InfoTarjetaRepo.Obtener(a => a.InfoTarjetaId == id);
            if (InfoTarjeta == null)
                return ResponseHelper.Fail<object>("InfoTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);

            await _InfoTarjetaRepo.Remover(InfoTarjeta);
            _logger.LogInformation("✅ InfoTarjeta ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "InfoTarjeta eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar InfoTarjeta ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<InfoTarjetaDto>> ActualizarInfoTarjetaAsync(int id, InfoTarjetaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<InfoTarjetaDto>("Datos inválidos para actualizar InfoTarjeta.", "InfoTarjeta");
            // Obtener entidad existente para mapeo y actualización
            var InfoTarjetaExistente = await _InfoTarjetaRepo.Obtener(a => a.InfoTarjetaId == id, tracked: true);
            if (InfoTarjetaExistente == null)
                return ResponseHelper.Fail<InfoTarjetaDto>("InfoTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<InfoTarjetaDto>(validation.Errors);

            _mapper.Map(updateDto, InfoTarjetaExistente);
            await _InfoTarjetaRepo.ActualizarInfoTarjeta(InfoTarjetaExistente);

            _logger.LogInformation("✅ InfoTarjeta ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<InfoTarjetaDto>(null, "InfoTarjeta actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar InfoTarjeta ID {Id}", id);
            return ResponseHelper.FailException<InfoTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<InfoTarjetaDto>> ActualizarParcialInfoTarjetaAsync(int id, JsonPatchDocument<InfoTarjetaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<InfoTarjetaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var InfoTarjetaExistente = await _InfoTarjetaRepo.Obtener(a => a.InfoTarjetaId == id, tracked: true);
            if (InfoTarjetaExistente == null)
                return ResponseHelper.Fail<InfoTarjetaDto>("InfoTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);
            // Aplicar cambios parciales al DTO y validarlos
            var dto = _mapper.Map<InfoTarjetaUpdateDto>(InfoTarjetaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<InfoTarjetaDto>(validation.Errors);

            _mapper.Map(dto, InfoTarjetaExistente);
            await _InfoTarjetaRepo.ActualizarInfoTarjeta(InfoTarjetaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al InfoTarjeta ID {Id}.", id);
            return ResponseHelper.Success<InfoTarjetaDto>(null, "InfoTarjeta actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al InfoTarjeta ID {Id}", id);
            return ResponseHelper.FailException<InfoTarjetaDto>(ex);
        }
    }
}
