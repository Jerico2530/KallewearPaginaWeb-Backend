using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Sucursal;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * SucursalService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con las sucursales.
 * Funcionalidades clave:
 * - Obtener todas las sucursales o una sucursal específica por ID.
 * - Crear, actualizar (completo o parcial) y eliminar sucursales.
 * - Exportar listado de sucursales a Excel, excluyendo datos sensibles.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de sucursales, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados en la creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class SucursalService : ISucursalService
{
    private readonly ISucursalRepositorio _SucursalRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<SucursalService> _logger;
    private readonly IValidator<SucursalCreateDto> _createValidator;
    private readonly IValidator<SucursalUpdateDto> _updateValidator;
    private readonly IValidator<SucursalUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public SucursalService(ISucursalRepositorio SucursalRepo, IMapper mapper, ILogger<SucursalService> logger, IValidator<SucursalCreateDto> createValidator, IValidator<SucursalUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<SucursalUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _SucursalRepo = SucursalRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _context = context;
        _excelGenericoService = excelGenericoService;

    }

    public async Task<ApiResponse<List<SucursalDto>>> ObtenerTodosLosSucursalAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Sucursals activos...");

            var Sucursals = await _SucursalRepo.ObtenerTodo();

            if (Sucursals == null || !Sucursals.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Sucursals registrados.");
                return ResponseHelper.Fail<List<SucursalDto>>(
                    new List<ErrorDetail> { new() { Campo = "Sucursal", Mensaje = "No se encontraron Sucursals registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var SucursalsDto = _mapper.Map<IEnumerable<SucursalDto>>(Sucursals).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Sucursals.", SucursalsDto.Count);
            return ResponseHelper.Success(SucursalsDto, "Sucursals obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Sucursals.");
            return ResponseHelper.FailException<List<SucursalDto>>(ex);
        }
    }

    public async Task<ApiResponse<SucursalDto>> ObtenerSucursalPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<SucursalDto>(validation.Errors);

            var Sucursal = await _SucursalRepo.Obtener(a => a.SucursalId == id);
            if (Sucursal == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Sucursal con ID {Id}.", id);
                return ResponseHelper.Fail<SucursalDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Sucursal con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<SucursalDto>(Sucursal);
            _logger.LogInformation("✅ Sucursal con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Sucursal encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Sucursal por ID {Id}", id);
            return ResponseHelper.FailException<SucursalDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelSucursalesAsync()
    {
        try
        {
            var sucursals = await _context.Sucursales.ToListAsync();
            var sucursalsDto = _mapper.Map<List<SucursalDto>>(sucursals);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                sucursalsDto, "Reporte de Sucursals", "Sucursals", excluir
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


    public async Task<ApiResponse<SucursalDto>> CrearSucursalAsync(SucursalCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<SucursalDto>("Datos inválidos para crear Sucursal.", "Sucursal");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<SucursalDto>(validation.Errors);

            var existe = await _SucursalRepo.Obtener(a => a.Locales.ToLower() == createDto.Locales.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<SucursalDto>("Ya existe un Sucursal con ese Locales.", "Locales", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Sucursal>(createDto);
            await _SucursalRepo.Crear(modelo);

            var dto = _mapper.Map<SucursalDto>(modelo);
            _logger.LogInformation("✅ Sucursal '{Locales}' creado correctamente.", dto.Locales);
            return ResponseHelper.Success(dto, "Sucursal creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Sucursal.");
            return ResponseHelper.FailException<SucursalDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarSucursalAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Sucursal = await _SucursalRepo.Obtener(a => a.SucursalId == id);
            if (Sucursal == null)
                return ResponseHelper.Fail<object>("Sucursal no encontrado.", "Id", HttpStatusCode.NotFound);

            await _SucursalRepo.Remover(Sucursal);
            _logger.LogInformation("✅ Sucursal ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Sucursal eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Sucursal ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<SucursalDto>> ActualizarSucursalAsync(int id, SucursalUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<SucursalDto>("Datos inválidos para actualizar Sucursal.", "Sucursal");

            var SucursalExistente = await _SucursalRepo.Obtener(a => a.SucursalId == id, tracked: true);
            if (SucursalExistente == null)
                return ResponseHelper.Fail<SucursalDto>("Sucursal no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<SucursalDto>(validation.Errors);

            _mapper.Map(updateDto, SucursalExistente);
            await _SucursalRepo.ActualizarSucursal(SucursalExistente);

            _logger.LogInformation("✅ Sucursal ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<SucursalDto>(null, "Sucursal actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Sucursal ID {Id}", id);
            return ResponseHelper.FailException<SucursalDto>(ex);
        }
    }

    public async Task<ApiResponse<SucursalDto>> ActualizarParcialSucursalAsync(int id, JsonPatchDocument<SucursalUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<SucursalDto>("Datos inválidos para la actualización parcial.", "Patch");

            var SucursalExistente = await _SucursalRepo.Obtener(a => a.SucursalId == id, tracked: true);
            if (SucursalExistente == null)
                return ResponseHelper.Fail<SucursalDto>("Sucursal no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<SucursalUpdateDto>(SucursalExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<SucursalDto>(validation.Errors);

            _mapper.Map(dto, SucursalExistente);
            await _SucursalRepo.ActualizarSucursal(SucursalExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Sucursal ID {Id}.", id);
            return ResponseHelper.Success<SucursalDto>(null, "Sucursal actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Sucursal ID {Id}", id);
            return ResponseHelper.FailException<SucursalDto>(ex);
        }
    }



}

