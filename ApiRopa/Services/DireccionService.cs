using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Direccion;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
/*
 * DireccionService
 *
 * Servicio de dominio encargado de gestionar la lógica principal de las direcciones.
 * Funcionalidades clave:
 * - Obtener todas las direcciones o una por ID.
 * - Crear, actualizar (completo o parcial) y eliminar direcciones.
 * - Exportar listado de direcciones a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de las direcciones y garantizar integridad en:
 * - Validación de datos
 * - Operaciones de CRUD
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class DireccionService : IDireccionService
{
    private readonly IDireccionRepositorio _DireccionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<DireccionService> _logger;
    private readonly IValidator<DireccionCreateDto> _createValidator;
    private readonly IValidator<DireccionUpdateDto> _updateValidator;
    private readonly IValidator<DireccionUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public DireccionService(IDireccionRepositorio DireccionRepo, IMapper mapper, ILogger<DireccionService> logger , IValidator<DireccionCreateDto> createValidator, IValidator<DireccionUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<DireccionUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _DireccionRepo = DireccionRepo;
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

    public async Task<ApiResponse<List<DireccionDto>>> ObtenerTodosLosDireccionAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Direccions activos...");

            var Direccions = await _DireccionRepo.ObtenerTodo();

            if (Direccions == null || !Direccions.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Direccions registrados.");
                return ResponseHelper.Fail<List<DireccionDto>>(
                    new List<ErrorDetail> { new() { Campo = "Direccions", Mensaje = "No se encontraron Direccions registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var DireccionsDto = _mapper.Map<IEnumerable<DireccionDto>>(Direccions).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Direccions.", DireccionsDto.Count);
            return ResponseHelper.Success(DireccionsDto, "Direccions obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Direccions.");
            return ResponseHelper.FailException<List<DireccionDto>>(ex);
        }
    }

    public async Task<ApiResponse<DireccionDto>> ObtenerDireccionPorIdAsync(int id)
    {
        try
        { 
            // Validación del ID
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DireccionDto>(validation.Errors);

            var Direccion = await _DireccionRepo.Obtener(a => a.DireccionId == id);
            if (Direccion == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Direccion con ID {Id}.", id);
                return ResponseHelper.Fail<DireccionDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Direccion con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<DireccionDto>(Direccion);
            _logger.LogInformation("✅ Direccion con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Direccion encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Direccion por ID {Id}", id);
            return ResponseHelper.FailException<DireccionDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelDireccionesAsync()
    {
        try
        {
            var direccions = await _context.Direcciones.ToListAsync();
            var direccionsDto = _mapper.Map<List<DireccionDto>>(direccions);


            // Exportación a Excel
            var bytes = await _excelGenericoService.ExportarExcel(
                direccionsDto, "Reporte de Direccions", "Direccions"
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


    public async Task<ApiResponse<DireccionDto>> CrearDireccionAsync(DireccionCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<DireccionDto>("Datos inválidos para crear Direccion.", "Direccion");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DireccionDto>(validation.Errors);


            var modelo = _mapper.Map<Direccion>(createDto);
            await _DireccionRepo.Crear(modelo);

            var dto = _mapper.Map<DireccionDto>(modelo);
            _logger.LogInformation("✅ Direccion '{Titulo}' creado correctamente.", dto.Departamento);
            return ResponseHelper.Success(dto, "Direccion creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Direccion.");
            return ResponseHelper.FailException<DireccionDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarDireccionAsync(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando direccion ID {Id}", id);

            ValidationResult validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Direccion = await _DireccionRepo.Obtener(p => p.DireccionId == id);
            if (Direccion == null)
                return ResponseHelper.Fail<object>("Direccion no encontrado.", "Id", HttpStatusCode.NotFound);

            await _DireccionRepo.Remover(Direccion);
            _logger.LogInformation("✅ direccion ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Direccion eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar direccion ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<DireccionDto>> ActualizarDireccionAsync(int id, DireccionUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("✏️ Iniciando actualización completa del direccion ID {Id}", id);

            if (updateDto == null )
                return ResponseHelper.Fail<DireccionDto>("Datos inválidos para actualizar Direccion.", "Direccion");

            var direccionExistente = await _DireccionRepo.Obtener(p => p.DireccionId == id, tracked: true);
            if (direccionExistente == null)
                return ResponseHelper.Fail<DireccionDto>("Direccion no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DireccionDto>(validation.Errors);

            _mapper.Map(updateDto, direccionExistente);
            await _DireccionRepo.ActualizarDireccion(direccionExistente);

            _logger.LogInformation("✅ direccion ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<DireccionDto>(null, "Direccion actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar direccion ID {Id}", id);
            return ResponseHelper.FailException<DireccionDto>(ex);
        }
    }

    public async Task<ApiResponse<DireccionDto>> ActualizarParcialDireccionAsync(int id, JsonPatchDocument<DireccionUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<DireccionDto>("Datos inválidos para la actualización parcial.", "Patch");

            var DireccionExistente = await _DireccionRepo.Obtener(a => a.DireccionId == id, tracked: true);
            if (DireccionExistente == null)
                return ResponseHelper.Fail<DireccionDto>("Direccion no encontrado.", "Id", HttpStatusCode.NotFound);
            // Aplicar cambios parciales al DTO y validar
            var dto = _mapper.Map<DireccionUpdateDto>(DireccionExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<DireccionDto>(validation.Errors);

            _mapper.Map(dto, DireccionExistente);
            await _DireccionRepo.ActualizarDireccion(DireccionExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Direccion ID {Id}.", id);
            return ResponseHelper.Success<DireccionDto>(null, "Direccion actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Direccion ID {Id}", id);
            return ResponseHelper.FailException<DireccionDto>(ex);
        }
    }
}
