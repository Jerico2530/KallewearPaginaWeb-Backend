using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Historia;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
/*
 * HistoriaService
 *
 * Servicio de dominio encargado de gestionar la lógica principal de las historias.
 * Funcionalidades clave:
 * - Obtener todas las historias o una por ID.
 * - Crear, actualizar (completo o parcial) y eliminar historias.
 * - Exportar listado de historias a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de las historias y garantizar integridad en:
 * - Existencia y unicidad de títulos
 * - Validación de datos
 * - Operaciones de CRUD
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class HistoriaService : IHistoriaService
{
    private readonly IHistoriaRepositorio _HistoriaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<HistoriaService> _logger;
    private readonly IValidator<HistoriaCreateDto> _createValidator;
    private readonly IValidator<HistoriaUpdateDto> _updateValidator;
    private readonly IValidator<HistoriaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public HistoriaService(IHistoriaRepositorio HistoriaRepo, IMapper mapper, ILogger<HistoriaService> logger, IValidator<HistoriaCreateDto> createValidator, IValidator<HistoriaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<HistoriaUpdateDto> patchValidator,
AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _HistoriaRepo = HistoriaRepo;
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

    public async Task<ApiResponse<List<HistoriaDto>>> ObtenerTodosLosHistoriaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Historias activos...");

            var Historias = await _HistoriaRepo.ObtenerTodo();

            if (Historias == null || !Historias.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Historias registrados.");
                return ResponseHelper.Fail<List<HistoriaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Historias", Mensaje = "No se encontraron Historias registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var HistoriasDto = _mapper.Map<IEnumerable<HistoriaDto>>(Historias).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Historias.", HistoriasDto.Count);
            return ResponseHelper.Success(HistoriasDto, "Historias obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Historias.");
            return ResponseHelper.FailException<List<HistoriaDto>>(ex);
        }
    }

    public async Task<ApiResponse<HistoriaDto>> ObtenerHistoriaPorIdAsync(int id)
    {
        try
        {
            // Validación de ID
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<HistoriaDto>(validation.Errors);

            var Historia = await _HistoriaRepo.Obtener(a => a.HistoriaId == id);
            if (Historia == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Historia con ID {Id}.", id);
                return ResponseHelper.Fail<HistoriaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Historia con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<HistoriaDto>(Historia);
            _logger.LogInformation("✅ Historia con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Historia encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Historia por ID {Id}", id);
            return ResponseHelper.FailException<HistoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelHistoriasAsync()
    {
        try
        {
            var historias = await _context.Historias.ToListAsync();
            var historiasDto = _mapper.Map<List<HistoriaDto>>(historias);

            // Generación de Excel mediante servicio auxiliar
            var bytes = await _excelGenericoService.ExportarExcel(
                historiasDto, "Reporte de Historias", "Historias"
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


    public async Task<ApiResponse<HistoriaDto>> CrearHistoriaAsync(HistoriaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<HistoriaDto>("Datos inválidos para crear Historia.", "Historia");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<HistoriaDto>(validation.Errors);

            // Verificar existencia de título duplicado
            var existe = await _HistoriaRepo.Obtener(a => a.Titulo.ToLower() == createDto.Titulo.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<HistoriaDto>("Ya existe un Historia con ese título.", "Titulo", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Historia>(createDto);
            await _HistoriaRepo.Crear(modelo);

            var dto = _mapper.Map<HistoriaDto>(modelo);
            _logger.LogInformation("✅ Historia '{Titulo}' creado correctamente.", dto.Titulo);
            return ResponseHelper.Success(dto, "Historia creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Historia.");
            return ResponseHelper.FailException<HistoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarHistoriaAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Historia = await _HistoriaRepo.Obtener(a => a.HistoriaId == id);
            if (Historia == null)
                return ResponseHelper.Fail<object>("Historia no encontrado.", "Id", HttpStatusCode.NotFound);

            await _HistoriaRepo.Remover(Historia);
            _logger.LogInformation("✅ Historia ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Historia eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Historia ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<HistoriaDto>> ActualizarHistoriaAsync(int id, HistoriaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<HistoriaDto>("Datos inválidos para actualizar Historia.", "Historia");
            // Obtener entidad existente para mapeo y actualización
            var HistoriaExistente = await _HistoriaRepo.Obtener(a => a.HistoriaId == id, tracked: true);
            if (HistoriaExistente == null)
                return ResponseHelper.Fail<HistoriaDto>("Historia no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<HistoriaDto>(validation.Errors);

            _mapper.Map(updateDto, HistoriaExistente);
            await _HistoriaRepo.ActualizarHistoria(HistoriaExistente);

            _logger.LogInformation("✅ Historia ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<HistoriaDto>(null, "Historia actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Historia ID {Id}", id);
            return ResponseHelper.FailException<HistoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<HistoriaDto>> ActualizarParcialHistoriaAsync(int id, JsonPatchDocument<HistoriaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<HistoriaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var HistoriaExistente = await _HistoriaRepo.Obtener(a => a.HistoriaId == id, tracked: true);
            if (HistoriaExistente == null)
                return ResponseHelper.Fail<HistoriaDto>("Historia no encontrado.", "Id", HttpStatusCode.NotFound);
            // Aplicar cambios parciales al DTO y validarlos
            var dto = _mapper.Map<HistoriaUpdateDto>(HistoriaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<HistoriaDto>(validation.Errors);

            _mapper.Map(dto, HistoriaExistente);
            await _HistoriaRepo.ActualizarHistoria(HistoriaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Historia ID {Id}.", id);
            return ResponseHelper.Success<HistoriaDto>(null, "Historia actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Historia ID {Id}", id);
            return ResponseHelper.FailException<HistoriaDto>(ex);
        }
    }
}

