using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.PermRol;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
/*
 * PermRolService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con la relación Permiso-Rol.
 * Funcionalidades clave:
 * - Obtener todos los PermRoles con detalles o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar PermRoles.
 * - Exportar listado de PermRoles a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de la relación Permiso-Rol, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados en creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class PermRolService : IPermRolService
{
    private readonly IPermRolRepositorio _PermRolRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PermRolService> _logger;
    private readonly IValidator<PermRolCreateDto> _createValidator;
    private readonly IValidator<PermRolUpdateDto> _updateValidator;
    private readonly IValidator<PermRolUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public PermRolService(IPermRolRepositorio PermRolRepo, IMapper mapper, ILogger<PermRolService> logger, IValidator<PermRolCreateDto> createValidator, IValidator<PermRolUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PermRolUpdateDto> patchValidator,
AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _PermRolRepo = PermRolRepo;
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

    public async Task<ApiResponse<List<PermRolDto>>> ObtenerPermRolConDetallesAsync()
    {

        try
        {
            _logger.LogInformation("📢 Obteniendo todos los PermRols (modo administrador)...");

            var PermRols = await _PermRolRepo.ObtenerPermRolConDetalles();

            if (PermRols == null || !PermRols.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron PermRols en la base de datos.");
                return ResponseHelper.Fail<List<PermRolDto>>(
                    new List<ErrorDetail> { new() { Campo = "PermRols", Mensaje = "No se encontraron PermRols registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var PermRolsDto = _mapper
                .Map<IEnumerable<PermRolDto>>(PermRols)
                .OrderByDescending(a => a.FechaRegistro)
                .ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} PermRols en modo administrador.", PermRolsDto.Count);
            return ResponseHelper.Success(PermRolsDto, "PermRols obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener todos los PermRols (modo administrador).");
            return ResponseHelper.FailException<List<PermRolDto>>(ex);
        }
    }

    public async Task<ApiResponse<PermRolDto>> ObtenerPermRolPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermRolDto>(validation.Errors);

            var PermRol = await _PermRolRepo.ObtenerPermRolConDetallesPorId(id);
            if (PermRol == null)
            {
                _logger.LogWarning("⚠️ No se encontró el PermRol con ID {Id}.", id);
                return ResponseHelper.Fail<PermRolDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el PermRol con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<PermRolDto>(PermRol);
            _logger.LogInformation("✅ PermRol con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "PermRol encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener PermRol por ID {Id}", id);
            return ResponseHelper.FailException<PermRolDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelPermRolesAsync()
    {
        try
        {
            var permRols = await _PermRolRepo.ObtenerPermRolConDetalles();
            var permRolsDto = _mapper.Map<List<PermRolDto>>(permRols);



            var bytes = await _excelGenericoService.ExportarExcel(
                permRolsDto, "Reporte de PermRols", "Permiso-Rol"
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


    public async Task<ApiResponse<PermRolDto>> CrearPermRolAsync(PermRolCreateDto createDto)
    {

        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<PermRolDto>("Datos inválidos para crear PermRol.", "PermRol");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermRolDto>(validation.Errors);

            var existente = await _PermRolRepo.Obtener(ur => ur.PermisoId == createDto.PermisoId && ur.RolId == createDto.RolId);
            if(existente != null)
                    return ResponseHelper.Fail<PermRolDto>("Ya existe un PermRol con ese PermisoId.", "PermisoId", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<PermRol>(createDto);
            await _PermRolRepo.Crear(modelo);

            var dto = _mapper.Map<PermRolDto>(modelo);
            _logger.LogInformation("✅ PermRol '{Titulo}' creado correctamente.", dto.PermisoId);
            return ResponseHelper.Success(dto, "PermRol creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear PermRol.");
            return ResponseHelper.FailException<PermRolDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarPermRolAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var PermRol = await _PermRolRepo.Obtener(a => a.PermRolId == id);
            if (PermRol == null)
                return ResponseHelper.Fail<object>("PermRol no encontrado.", "Id", HttpStatusCode.NotFound);

            await _PermRolRepo.Remover(PermRol);
            _logger.LogInformation("✅ PermRol ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "PermRol eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar PermRol ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<PermRolDto>> ActualizarPermRolAsync(int id, PermRolUpdateDto updateDto)
    {

        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<PermRolDto>("Datos inválidos para actualizar PermRol.", "PermRol");

            var PermRolExistente = await _PermRolRepo.Obtener(a => a.PermRolId == id, tracked: true);
            if (PermRolExistente == null)
                return ResponseHelper.Fail<PermRolDto>("PermRol no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PermRolDto>(validation.Errors);

            _mapper.Map(updateDto, PermRolExistente);
            await _PermRolRepo.ActualizarPermRol(PermRolExistente);

            _logger.LogInformation("✅ PermRol ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<PermRolDto>(null, "PermRol actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar PermRol ID {Id}", id);
            return ResponseHelper.FailException<PermRolDto>(ex);
        }
    }
}

