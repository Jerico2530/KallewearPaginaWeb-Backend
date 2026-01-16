using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
/*
 * DetalleTarjetaService
 *
 * Servicio de dominio encargado de gestionar la lógica principal de los DetalleTarjetas.
 * Funcionalidades clave:
 * - Obtener todos los DetalleTarjetas o uno por ID.
 * - Crear, actualizar (completo o parcial) y eliminar DetalleTarjetas.
 * - Exportar listado de DetalleTarjetas a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de los DetalleTarjetas y garantizar integridad en:
 * - Existencia y unicidad de números de tarjeta
 * - Validación de datos
 * - Operaciones de CRUD
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class DetalleTarjetaService : IDetalleTarjetaService
{
    private readonly IDetalleTarjetaRepositorio _DetalleTarjetaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DetalleTarjetaService> _logger;
    private readonly IValidator<DetalleTarjetaCreateDto> _createValidator;
    private readonly IValidator<DetalleTarjetaUpdateDto> _updateValidator;
    private readonly IValidator<DetalleTarjetaUpdateDto> _patchValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;


    public DetalleTarjetaService(IDetalleTarjetaRepositorio DetalleTarjetaRepo, IMapper mapper, ILogger<DetalleTarjetaService> logger , IValidator<DetalleTarjetaCreateDto> createValidator, IValidator<DetalleTarjetaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<DetalleTarjetaUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _DetalleTarjetaRepo = DetalleTarjetaRepo;
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

    public async Task<ApiResponse<List<DetalleTarjetaDto>>> ObtenerTodosLosDetalleTarjetaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los detalleTarjetas activos...");

            var DetalleTarjetaes = await _DetalleTarjetaRepo.ObtenerTodo();
            // Validar si hay resultados
            if (DetalleTarjetaes == null || !DetalleTarjetaes.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron detalleTarjetas registrados.");
                return ResponseHelper.Fail<List<DetalleTarjetaDto>>(
                        new List<ErrorDetail> { new() { Campo = "DetalleTarjetas", Mensaje = "No se encontraron DetalleTarjetas registrados." } },
                        HttpStatusCode.NotFound
                    );
            }
            // Mapear entidades a DTOs
            var DetalleTarjetaesDto = _mapper.Map<IEnumerable<DetalleTarjetaDto>>(DetalleTarjetaes).ToList();

            _logger.LogInformation("✅ {Count} detalles de tarjeta obtenidos exitosamente.", DetalleTarjetaesDto.Count());
            return ResponseHelper.Success(DetalleTarjetaesDto, "DetalleTarjetas obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener detalleTarjetas.");
            return ResponseHelper.FailException<List<DetalleTarjetaDto>>(ex);
        }
    }

    public async Task<ApiResponse<DetalleTarjetaDto>> ObtenerDetalleTarjetaPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo DetalleTarjeta con ID {Id}", id);

            // Validación de ID usando FluentValidation
            ValidationResult validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DetalleTarjetaDto>(validation.Errors);


            var DetalleTarjeta = await _DetalleTarjetaRepo.Obtener(p => p.DetalleTarjetaId == id);

            if (DetalleTarjeta == null)
            {
                _logger.LogWarning("⚠️ No se encontró el DetalleTarjeta con ID {Id}.", id);
                return ResponseHelper.Fail<DetalleTarjetaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el DetalleTarjeta con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<DetalleTarjetaDto>(DetalleTarjeta);
            _logger.LogInformation("✅ detalleTarjeta ID {Id} obtenido exitosamente.", id);
            return ResponseHelper.Success(dto, "DetalleTarjeta encontrado");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener detalleTarjeta con ID {Id}", id);
            return ResponseHelper.FailException<DetalleTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelDetalleTarjetasAsync()
    {
        try
        {
            var detalleTarjetas = await _context.DetalleTarjetas.ToListAsync();
            var detalleTarjetasDto = _mapper.Map<List<DetalleTarjetaDto>>(detalleTarjetas);

            // Excluir propiedades sensibles o imágenes al exportar Excel
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                detalleTarjetasDto, "Reporte de DetalleTarjetas", "DetalleTarjetas", excluir
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


    public async Task<ApiResponse<DetalleTarjetaDto>> CrearDetalleTarjetaAsync(DetalleTarjetaCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("🟢 Iniciando creación de detalleTarjeta: {NumeroTarjeta}", createDto?.NumeroTarjeta);

            if (createDto == null)
                return ResponseHelper.Fail<DetalleTarjetaDto>("Datos inválidos para crear DetalleTarjeta.", "DetalleTarjeta");

            ValidationResult validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DetalleTarjetaDto>(validation.Errors);
            // Validar duplicado por número de tarjeta
            var existe = await _DetalleTarjetaRepo.Obtener(p => p.NumeroTarjeta.ToLower() == createDto.NumeroTarjeta.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<DetalleTarjetaDto>("Ya existe un DetalleTarjeta con ese título.", "Titulo", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<DetalleTarjeta>(createDto);
            await _DetalleTarjetaRepo.Crear(modelo);

            var dto = _mapper.Map<DetalleTarjetaDto>(modelo);
            _logger.LogInformation("✅ DetalleTarjeta '{Titulo}' creado correctamente.", dto.NumeroTarjeta);
            return ResponseHelper.Success(dto, "DetalleTarjeta creado correctamente", HttpStatusCode.Created);
        }


        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear detalleTarjeta: {@CreateDto}", createDto);
            return ResponseHelper.FailException<DetalleTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarDetalleTarjetaAsync(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando detalleTarjeta ID {Id}", id);

            ValidationResult validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var DetalleTarjeta = await _DetalleTarjetaRepo.Obtener(p => p.DetalleTarjetaId == id);
            if (DetalleTarjeta == null)
            {
                _logger.LogWarning("⚠️ No se encontró detalleTarjeta ID {Id}", id);
                return ResponseHelper.Fail<object>("DetalleTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);
            }

            await _DetalleTarjetaRepo.Remover(DetalleTarjeta);
            _logger.LogInformation("✅ detalleTarjeta ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "DetalleTarjeta eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar detalleTarjeta ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<DetalleTarjetaDto>> ActualizarDetalleTarjetaAsync(int id, DetalleTarjetaUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("✏️ Iniciando actualización completa del detalleTarjeta ID {Id}", id);

            if (updateDto == null)
                return ResponseHelper.Fail<DetalleTarjetaDto>("Datos inválidos para actualizar DetalleTarjeta.", "DetalleTarjeta");

            var detalleTarjetaExistente = await _DetalleTarjetaRepo.Obtener(p => p.DetalleTarjetaId == id, tracked: true);
            if (detalleTarjetaExistente == null)
                return ResponseHelper.Fail<DetalleTarjetaDto>("DetalleTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DetalleTarjetaDto>(validation.Errors);

            _mapper.Map(updateDto, detalleTarjetaExistente);
            await _DetalleTarjetaRepo.ActualizarDetalleTarjeta(detalleTarjetaExistente);

            _logger.LogInformation("✅ detalleTarjeta ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<DetalleTarjetaDto>(null, "DetalleTarjeta actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar detalleTarjeta ID {Id}", id);
            return ResponseHelper.FailException<DetalleTarjetaDto>(ex);
        }
    }

    public async Task<ApiResponse<DetalleTarjetaDto>> ActualizarParcialDetalleTarjetaAsync(int id, JsonPatchDocument<DetalleTarjetaUpdateDto> patchDto)
    {
        try
        {
            _logger.LogInformation("Aplicando PATCH al detalleTarjeta ID {Id}", id);

            if (patchDto == null || id == 0)
                return ResponseHelper.Fail<DetalleTarjetaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var DetalleTarjeta = await _DetalleTarjetaRepo.Obtener(v => v.DetalleTarjetaId == id, tracked: false);

            if (DetalleTarjeta == null)
                return ResponseHelper.Fail<DetalleTarjetaDto>("DetalleTarjeta no encontrado.", "Id", HttpStatusCode.NotFound);

            var DetalleTarjetaDto = _mapper.Map<DetalleTarjetaUpdateDto>(DetalleTarjeta);
            patchDto.ApplyTo(DetalleTarjetaDto);

            var validation = await _updateValidator.ValidateAsync(DetalleTarjetaDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DetalleTarjetaDto>(validation.Errors);

            _mapper.Map(DetalleTarjetaDto, DetalleTarjeta);
            await _DetalleTarjetaRepo.ActualizarDetalleTarjeta(DetalleTarjeta);
            _logger.LogInformation("✅ PATCH aplicado correctamente al detalleTarjeta ID {Id}.", id);
            return ResponseHelper.Success<DetalleTarjetaDto>(null, "DetalleTarjeta actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al detalleTarjeta ID {Id}", id);

            return ResponseHelper.FailException<DetalleTarjetaDto>(ex);
        }
    }
}
