using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Permiso;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
/*
 * PermisoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con permisos de usuarios.
 * Funcionalidades clave:
 * - Obtener todos los permisos o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar permisos.
 * - Exportar listado de permisos a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de permisos, asegurando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados en creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones de permisos se realicen correctamente, manteniendo
 * el código limpio, profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class PermisoService : IPermisoService
{
    private readonly IPermisoRepositorio _PermisoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PermisoService> _logger;
    private readonly IValidator<PermisoCreateDto> _createValidator;
    private readonly IValidator<PermisoUpdateDto> _updateValidator;
    private readonly IValidator<PermisoUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;



    public PermisoService(IPermisoRepositorio PermisoRepo, IMapper mapper, ILogger<PermisoService> logger, IValidator<PermisoCreateDto> createValidator, IValidator<PermisoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PermisoUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _PermisoRepo = PermisoRepo;
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

    public async Task<ApiResponse<List<PermisoDto>>> ObtenerTodosLosPermisoAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Permisos activos...");

            var Permisos = await _PermisoRepo.ObtenerTodo();

            if (Permisos == null || !Permisos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Permisos registrados.");
                return ResponseHelper.Fail<List<PermisoDto>>(
                    new List<ErrorDetail> { new() { Campo = "Permisos", Mensaje = "No se encontraron Permisos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var PermisosDto = _mapper.Map<IEnumerable<PermisoDto>>(Permisos).OrderBy(a => a.NombrePermiso).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Permisos.", PermisosDto.Count);
            return ResponseHelper.Success(PermisosDto, "Permisos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Permisos.");
            return ResponseHelper.FailException<List<PermisoDto>>(ex);
        }
    }

    public async Task<ApiResponse<PermisoDto>> ObtenerPermisoPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermisoDto>(validation.Errors);

            var Permiso = await _PermisoRepo.Obtener(a => a.PermisoId == id);
            if (Permiso == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Permiso con ID {Id}.", id);
                return ResponseHelper.Fail<PermisoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Permiso con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<PermisoDto>(Permiso);
            _logger.LogInformation("✅ Permiso con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Permiso encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Permiso por ID {Id}", id);
            return ResponseHelper.FailException<PermisoDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelPermisosAsync()
    {
        try
        {
            var permisos = await _context.Permisos.ToListAsync();
            var permisosDto = _mapper.Map<List<PermisoDto>>(permisos);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                permisosDto, "Reporte de Permisos", "Permisos", excluir
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


    public async Task<ApiResponse<PermisoDto>> CrearPermisoAsync(PermisoCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<PermisoDto>("Datos inválidos para crear Permiso.", "Permiso");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermisoDto>(validation.Errors);

            var existe = await _PermisoRepo.Obtener(a => a.NombrePermiso.ToLower() == createDto.NombrePermiso.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<PermisoDto>("Ya existe un Permiso con ese NombrePermiso.", "NombrePermiso", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Permiso>(createDto);
            await _PermisoRepo.Crear(modelo);

            var dto = _mapper.Map<PermisoDto>(modelo);
            _logger.LogInformation("✅ Permiso '{NombrePermiso}' creado correctamente.", dto.NombrePermiso);
            return ResponseHelper.Success(dto, "Permiso creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Permiso.");
            return ResponseHelper.FailException<PermisoDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarPermisoAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Permiso = await _PermisoRepo.Obtener(a => a.PermisoId == id);
            if (Permiso == null)
                return ResponseHelper.Fail<object>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

            await _PermisoRepo.Remover(Permiso);
            _logger.LogInformation("✅ Permiso ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Permiso eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Permiso ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<PermisoDto>> ActualizarPermisoAsync(int id, PermisoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<PermisoDto>("Datos inválidos para actualizar Permiso.", "Permiso");

            var PermisoExistente = await _PermisoRepo.Obtener(a => a.PermisoId == id, tracked: true);
            if (PermisoExistente == null)
                return ResponseHelper.Fail<PermisoDto>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermisoDto>(validation.Errors);

            _mapper.Map(updateDto, PermisoExistente);
            await _PermisoRepo.ActualizarPermiso(PermisoExistente);

            _logger.LogInformation("✅ Permiso ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<PermisoDto>(null, "Permiso actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Permiso ID {Id}", id);
            return ResponseHelper.FailException<PermisoDto>(ex);
        }
    }



    public async Task<ApiResponse<PermisoDto>> ActualizarParcialPermisoAsync(int id, JsonPatchDocument<PermisoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<PermisoDto>("Datos inválidos para la actualización parcial.", "Patch");

            var PermisoExistente = await _PermisoRepo.Obtener(a => a.PermisoId == id, tracked: true);
            if (PermisoExistente == null)
                return ResponseHelper.Fail<PermisoDto>("Permiso no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<PermisoUpdateDto>(PermisoExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermisoDto>(validation.Errors);

            _mapper.Map(dto, PermisoExistente);
            await _PermisoRepo.ActualizarPermiso(PermisoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Permiso ID {Id}.", id);
            return ResponseHelper.Success<PermisoDto>(null, "Permiso actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Permiso ID {Id}", id);
            return ResponseHelper.FailException<PermisoDto>(ex);
        }

    }
}

