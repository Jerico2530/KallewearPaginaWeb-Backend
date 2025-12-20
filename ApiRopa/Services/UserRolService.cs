using ApiRopa;
using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.UserRol;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * UserRolService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con roles de usuario.
 * Funcionalidades clave:
 * - Obtener todos los roles de usuario con detalles o uno específico por ID.
 * - Crear, actualizar y eliminar roles de usuario.
 * - Exportar listado de roles de usuario a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de roles de usuario, asegurando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar inconsistencias en la asignación de roles y mantener la integridad de la información.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * garantizando que las operaciones se realicen correctamente y manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

public class UserRolService : IUserRolService
{
    private readonly IUserRolRepositorio _UserRolRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<UserRolService> _logger;
    private readonly IValidator<UserRolCreateDto> _createValidator;
    private readonly IValidator<UserRolUpdateDto> _updateValidator;
    private readonly IValidator<UserRolUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public UserRolService(IUserRolRepositorio userRolRepo, IMapper mapper, ILogger<UserRolService> logger, IValidator<UserRolCreateDto> createValidator, IValidator<UserRolUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<UserRolUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _UserRolRepo = userRolRepo;
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

    public async Task<ApiResponse<List<UserRolDto>>> ObtenerUserRolesConDetallesAsync()
    {

        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los UserRols activos...");

            var UserRols = await _UserRolRepo.ObtenerUserRolesConDetalles();

            if (UserRols == null || !UserRols.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron UserRols registrados.");
                return ResponseHelper.Fail<List<UserRolDto>>(
                    new List<ErrorDetail> { new() { Campo = "UserRols", Mensaje = "No se encontraron UserRols registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var UserRolsDto = _mapper.Map<IEnumerable<UserRolDto>>(UserRols).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} UserRols.", UserRolsDto.Count);
            return ResponseHelper.Success(UserRolsDto, "UserRols obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener UserRols.");
            return ResponseHelper.FailException<List<UserRolDto>>(ex);
        }
    }

    public async Task<ApiResponse<UserRolDto>> ObtenerUserRolPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UserRolDto>(validation.Errors);

            var UserRol = await _UserRolRepo.ObtenerUserRolConDetallesPorId(id);
            if (UserRol == null)
            {
                _logger.LogWarning("⚠️ No se encontró el UserRol con ID {Id}.", id);
                return ResponseHelper.Fail<UserRolDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el UserRol con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<UserRolDto>(UserRol);
            _logger.LogInformation("✅ UserRol con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "UserRol encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener UserRol por ID {Id}", id);
            return ResponseHelper.FailException<UserRolDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelUserRolesAsync()
    {
        try
        {
            var userRoles = await _UserRolRepo.ObtenerUserRolesConDetalles();
            var userRolDto = _mapper.Map<List<UserRolDto>>(userRoles);



            var bytes = await _excelGenericoService.ExportarExcel(
                userRolDto, "Reporte de UserRoles", "UserRoles"
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


    public async Task<ApiResponse<UserRolDto>> CrearUserRolAsync(UserRolCreateDto createDto)
    {

        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<UserRolDto>("Datos inválidos para crear UserRol.", "UserRol");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UserRolDto>(validation.Errors);

  

            var modelo = _mapper.Map<UserRol>(createDto);
            await _UserRolRepo.Crear(modelo);

            var dto = _mapper.Map<UserRolDto>(modelo);
            _logger.LogInformation("✅ UserRol '{UsuarioId}' creado correctamente.", dto.UsuarioId);
            return ResponseHelper.Success(dto, "UserRol creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear UserRol.");
            return ResponseHelper.FailException<UserRolDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarUserRolAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var UserRol = await _UserRolRepo.Obtener(a => a.UserRolId == id);
            if (UserRol == null)
                return ResponseHelper.Fail<object>("UserRol no encontrado.", "Id", HttpStatusCode.NotFound);

            await _UserRolRepo.Remover(UserRol);
            _logger.LogInformation("✅ UserRol ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "UserRol eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar UserRol ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<UserRolDto>> ActualizarUserRolAsync(int id, UserRolUpdateDto updateDto)
    {

        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<UserRolDto>("Datos inválidos para actualizar UserRol.", "UserRol");

            var UserRolExistente = await _UserRolRepo.Obtener(a => a.UserRolId == id, tracked: true);
            if (UserRolExistente == null)
                return ResponseHelper.Fail<UserRolDto>("UserRol no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UserRolDto>(validation.Errors);

            _mapper.Map(updateDto, UserRolExistente);
            await _UserRolRepo.ActualizarUserRol(UserRolExistente);

            _logger.LogInformation("✅ UserRol ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<UserRolDto>(null, "UserRol actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar UserRol ID {Id}", id);
            return ResponseHelper.FailException<UserRolDto>(ex);
        }
    }
}
