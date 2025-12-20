using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Moneda;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * MonedaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con monedas.
 * Funcionalidades clave:
 * - Obtener todas las monedas o una por ID.
 * - Crear, actualizar (completo o parcial) y eliminar monedas.
 * - Exportar listado de monedas a Excel.
 * - Validar datos mediante FluentValidation.
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de monedas, asegurando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados en creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * garantizando operaciones correctas y manteniendo el código limpio, profesional
 * y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class MonedaService : IMonedaService
{
    private readonly IMonedaRepositorio _MonedaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<MonedaService> _logger;
    private readonly IValidator<MonedaCreateDto> _createValidator;
    private readonly IValidator<MonedaUpdateDto> _updateValidator;
    private readonly IValidator<MonedaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;



    public MonedaService(IMonedaRepositorio MonedaRepo, IMapper mapper, ILogger<MonedaService> logger , IValidator<MonedaCreateDto> createValidator, IValidator<MonedaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<MonedaUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _MonedaRepo = MonedaRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _context = context;
        _excelGenericoService = excelGenericoService;
        _patchValidator = patchValidator;

    }

    public async Task<ApiResponse<List<MonedaDto>>> ObtenerTodosLosMonedaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Monedas activos...");
            // Obtener todas las monedas desde el repositorio
            var Monedas = await _MonedaRepo.ObtenerTodo();

            if (Monedas == null || !Monedas.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Monedas registrados.");
                return ResponseHelper.Fail<List<MonedaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Monedas", Mensaje = "No se encontraron Monedas registrados." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidades a DTO y ordenar por código
            var MonedasDto = _mapper.Map<IEnumerable<MonedaDto>>(Monedas).OrderBy(a => a.Codigo).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Monedas.", MonedasDto.Count);
            return ResponseHelper.Success(MonedasDto, "Monedas obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Monedas.");
            return ResponseHelper.FailException<List<MonedaDto>>(ex);
        }
    }

    public async Task<ApiResponse<MonedaDto>> ObtenerMonedaPorIdAsync(int id)
    {
        try
        {
            // Validar ID de entrada
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MonedaDto>(validation.Errors);

            var Moneda = await _MonedaRepo.Obtener(a => a.MonedaId == id);
            if (Moneda == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Moneda con ID {Id}.", id);
                return ResponseHelper.Fail<MonedaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Moneda con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidad a DTO
            var dto = _mapper.Map<MonedaDto>(Moneda);
            _logger.LogInformation("✅ Moneda con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Moneda encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Moneda por ID {Id}", id);
            return ResponseHelper.FailException<MonedaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelMonedasAsync()
    {
        try
        {
            // Obtener todas las monedas y mapear a DTO
            var monedas = await _context.Monedas.ToListAsync();
            var monedasDto = _mapper.Map<List<MonedaDto>>(monedas);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                monedasDto, "Reporte de Monedas", "Monedas", excluir
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


    public async Task<ApiResponse<MonedaDto>> CrearMonedaAsync(MonedaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<MonedaDto>("Datos inválidos para crear Moneda.", "Moneda");
            // Validar DTO de creación
            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MonedaDto>(validation.Errors);
            // Comprobar existencia de moneda con mismo código
            var existe = await _MonedaRepo.Obtener(a => a.Codigo.ToLower() == createDto.Codigo.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<MonedaDto>("Ya existe un Moneda con ese título.", "Codigo", HttpStatusCode.Conflict);
            // Mapear DTO a entidad y guardar
            var modelo = _mapper.Map<Moneda>(createDto);
            await _MonedaRepo.Crear(modelo);

            var dto = _mapper.Map<MonedaDto>(modelo);
            _logger.LogInformation("✅ Moneda '{Codigo}' creado correctamente.", dto.Codigo);
            return ResponseHelper.Success(dto, "Moneda creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Moneda.");
            return ResponseHelper.FailException<MonedaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarMonedaAsync(int id)
    {
        try
        {
            // Validar ID antes de eliminar
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Moneda = await _MonedaRepo.Obtener(a => a.MonedaId == id);
            if (Moneda == null)
                return ResponseHelper.Fail<object>("Moneda no encontrado.", "Id", HttpStatusCode.NotFound);
            // Eliminar entidad
            await _MonedaRepo.Remover(Moneda);
            _logger.LogInformation("✅ Moneda ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Moneda eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Moneda ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<MonedaDto>> ActualizarMonedaAsync(int id, MonedaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<MonedaDto>("Datos inválidos para actualizar Moneda.", "Moneda");
            // Obtener entidad existente con tracking
            var MonedaExistente = await _MonedaRepo.Obtener(a => a.MonedaId == id, tracked: true);
            if (MonedaExistente == null)
                return ResponseHelper.Fail<MonedaDto>("Moneda no encontrado.", "Id", HttpStatusCode.NotFound);
            // Validar DTO de actualización
            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MonedaDto>(validation.Errors);
            // Mapear cambios y guardar
            _mapper.Map(updateDto, MonedaExistente);
            await _MonedaRepo.ActualizarMoneda(MonedaExistente);

            _logger.LogInformation("✅ Moneda ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<MonedaDto>(null, "Moneda actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Moneda ID {Id}", id);
            return ResponseHelper.FailException<MonedaDto>(ex);
        }
    }

    public async Task<ApiResponse<MonedaDto>> ActualizarParcialMonedaAsync(int id, JsonPatchDocument<MonedaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<MonedaDto>("Datos inválidos para la actualización parcial.", "Patch");
            // Obtener entidad existente con tracking
            var MonedaExistente = await _MonedaRepo.Obtener(a => a.MonedaId == id, tracked: true);
            if (MonedaExistente == null)
                return ResponseHelper.Fail<MonedaDto>("Moneda no encontrado.", "Id", HttpStatusCode.NotFound);
            // Aplicar patch al DTO y validar
            var dto = _mapper.Map<MonedaUpdateDto>(MonedaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MonedaDto>(validation.Errors);
            // Mapear cambios y actualizar
            _mapper.Map(dto, MonedaExistente);
            await _MonedaRepo.ActualizarMoneda(MonedaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Moneda ID {Id}.", id);
            return ResponseHelper.Success<MonedaDto>(null, "Moneda actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Moneda ID {Id}", id);
            return ResponseHelper.FailException<MonedaDto>(ex);
        }
    }
}




