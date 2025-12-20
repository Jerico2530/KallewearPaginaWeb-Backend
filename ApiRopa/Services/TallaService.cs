using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Talla;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;
/*
 * TallaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con tallas de productos.
 * Funcionalidades clave:
 * - Obtener todas las tallas o una talla específica por ID.
 * - Crear, actualizar (completo o parcial) y eliminar tallas.
 * - Exportar listado de tallas a Excel, excluyendo información sensible.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de tallas, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados en la creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class TallaService : ITallaService
{
    private readonly ITallaRepositorio _TallaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<TallaService> _logger;
    private readonly IValidator<TallaCreateDto> _createValidator;
    private readonly IValidator<TallaUpdateDto> _updateValidator;
    private readonly IValidator<TallaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;

    public TallaService(ITallaRepositorio TallaRepo, IMapper mapper, ILogger<TallaService> logger, IValidator<TallaCreateDto> createValidator, IValidator<TallaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<TallaUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _TallaRepo = TallaRepo;
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

    public async Task<ApiResponse<List<TallaDto>>> ObtenerTodosLosTallaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Tallas activos...");

            var Tallas = await _TallaRepo.ObtenerTodo();

            if (Tallas == null || !Tallas.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Tallas registrados.");
                return ResponseHelper.Fail<List<TallaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Tallas", Mensaje = "No se encontraron Tallas registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var TallasDto = _mapper.Map<IEnumerable<TallaDto>>(Tallas).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Tallas.", TallasDto.Count);
            return ResponseHelper.Success(TallasDto, "Tallas obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Tallas.");
            return ResponseHelper.FailException<List<TallaDto>>(ex);
        }
    }

    public async Task<ApiResponse<TallaDto>> ObtenerTallaPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TallaDto>(validation.Errors);

            var Talla = await _TallaRepo.Obtener(a => a.TallaId == id);
            if (Talla == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Talla con ID {Id}.", id);
                return ResponseHelper.Fail<TallaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Talla con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<TallaDto>(Talla);
            _logger.LogInformation("✅ Talla con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Talla encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Talla por ID {Id}", id);
            return ResponseHelper.FailException<TallaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelTallasAsync()
    {
        try
        {
            var tallas = await _context.Tallas.ToListAsync();
            var tallasDto = _mapper.Map<List<TallaDto>>(tallas);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                tallasDto, "Reporte de Tallas", "Tallas", excluir
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


    public async Task<ApiResponse<TallaDto>> CrearTallaAsync(TallaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<TallaDto>("Datos inválidos para crear Talla.", "Talla");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TallaDto>(validation.Errors);

            var existe = await _TallaRepo.Obtener(a => a.TipoTalla.ToLower() == createDto.TipoTalla.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<TallaDto>("Ya existe un Talla con ese TipoTalla.", "TipoTalla", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Talla>(createDto);
            await _TallaRepo.Crear(modelo);

            var dto = _mapper.Map<TallaDto>(modelo);
            _logger.LogInformation("✅ Talla '{TipoTalla}' creado correctamente.", dto.TipoTalla);
            return ResponseHelper.Success(dto, "Talla creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Talla.");
            return ResponseHelper.FailException<TallaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarTallaAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Talla = await _TallaRepo.Obtener(a => a.TallaId == id);
            if (Talla == null)
                return ResponseHelper.Fail<object>("Talla no encontrado.", "Id", HttpStatusCode.NotFound);

            await _TallaRepo.Remover(Talla);
            _logger.LogInformation("✅ Talla ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Talla eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Talla ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<TallaDto>> ActualizarTallaAsync(int id, TallaUpdateDto updateDto)
    {

        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<TallaDto>("Datos inválidos para actualizar Talla.", "Talla");

            var TallaExistente = await _TallaRepo.Obtener(a => a.TallaId == id, tracked: true);
            if (TallaExistente == null)
                return ResponseHelper.Fail<TallaDto>("Talla no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TallaDto>(validation.Errors);

            _mapper.Map(updateDto, TallaExistente);
            await _TallaRepo.ActualizarTalla(TallaExistente);

            _logger.LogInformation("✅ Talla ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<TallaDto>(null, "Talla actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Talla ID {Id}", id);
            return ResponseHelper.FailException<TallaDto>(ex);
        }
    }

    public async Task<ApiResponse<TallaDto>> ActualizarParcialTallaAsync(int id, JsonPatchDocument<TallaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<TallaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var TallaExistente = await _TallaRepo.Obtener(a => a.TallaId == id, tracked: true);
            if (TallaExistente == null)
                return ResponseHelper.Fail<TallaDto>("Talla no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<TallaUpdateDto>(TallaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TallaDto>(validation.Errors);

            _mapper.Map(dto, TallaExistente);
            await _TallaRepo.ActualizarTalla(TallaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Talla ID {Id}.", id);
            return ResponseHelper.Success<TallaDto>(null, "Talla actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Talla ID {Id}", id);
            return ResponseHelper.FailException<TallaDto>(ex);
        }
    }

}

