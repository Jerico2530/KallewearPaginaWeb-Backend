using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Genero;
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
 * GeneroService
 *
 * Servicio de dominio encargado de gestionar la lógica principal de los géneros en el sistema.
 *
 * Funcionalidades clave:
 * - Obtener todos los géneros o uno por ID.
 * - Crear, actualizar (completo o parcial) y eliminar géneros.
 * - Exportar listado de géneros a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (mapeo y generación de Excel).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de los géneros, garantizando integridad en:
 * - Existencia y unicidad de tipos de género.
 * - Validación de datos de entrada.
 * - Operaciones de CRUD.
 *
 * Este servicio actúa como capa intermedia entre los controladores y los repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class GeneroService : IGeneroService
{
    private readonly IGeneroRepositorio _GeneroRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<GeneroService> _logger;
    private readonly IValidator<GeneroCreateDto> _createValidator;
    private readonly IValidator<GeneroUpdateDto> _updateValidator;
    private readonly IValidator<GeneroUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public GeneroService(IGeneroRepositorio GeneroRepo, IMapper mapper, ILogger<GeneroService> logger, IValidator<GeneroCreateDto> createValidator, IValidator<GeneroUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<GeneroUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _GeneroRepo = GeneroRepo;
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

    public async Task<ApiResponse<List<GeneroDto>>> ObtenerTodosLosGeneroAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Generos activos...");

            var Generos = await _GeneroRepo.ObtenerTodo();

            if (Generos == null || !Generos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Generos registrados.");
                return ResponseHelper.Fail<List<GeneroDto>>(
                    new List<ErrorDetail> { new() { Campo = "Generos", Mensaje = "No se encontraron Generos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var GenerosDto = _mapper.Map<IEnumerable<GeneroDto>>(Generos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Generos.", GenerosDto.Count);
            return ResponseHelper.Success(GenerosDto, "Generos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Generos.");
            return ResponseHelper.FailException<List<GeneroDto>>(ex);
        }
    }

    public async Task<ApiResponse<GeneroDto>> ObtenerGeneroPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<GeneroDto>(validation.Errors);

            var Genero = await _GeneroRepo.Obtener(a => a.GeneroId == id);
            if (Genero == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Genero con ID {Id}.", id);
                return ResponseHelper.Fail<GeneroDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Genero con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<GeneroDto>(Genero);
            _logger.LogInformation("✅ Genero con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Genero encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Genero por ID {Id}", id);
            return ResponseHelper.FailException<GeneroDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelGenerosAsync()
    {
        try
        {
            var generos = await _context.Generos.ToListAsync();
            var generosDto = _mapper.Map<List<GeneroDto>>(generos);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                generosDto, "Reporte de Generos", "Generos", excluir
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



    public async Task<ApiResponse<GeneroDto>> CrearGeneroAsync(GeneroCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<GeneroDto>("Datos inválidos para crear Genero.", "Genero");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<GeneroDto>(validation.Errors);

            var existe = await _GeneroRepo.Obtener(a => a.Tipo.ToLower() == createDto.Tipo.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<GeneroDto>("Ya existe un Genero con ese Tipo.", "Tipo", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Genero>(createDto);
            await _GeneroRepo.Crear(modelo);

            var dto = _mapper.Map<GeneroDto>(modelo);
            _logger.LogInformation("✅ Genero '{Titulo}' creado correctamente.", dto.Tipo);
            return ResponseHelper.Success(dto, "Genero creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Genero.");
            return ResponseHelper.FailException<GeneroDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarGeneroAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Genero = await _GeneroRepo.Obtener(a => a.GeneroId == id);
            if (Genero == null)
                return ResponseHelper.Fail<object>("Genero no encontrado.", "Id", HttpStatusCode.NotFound);

            await _GeneroRepo.Remover(Genero);
            _logger.LogInformation("✅ Genero ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Genero eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Genero ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<GeneroDto>> ActualizarGeneroAsync(int id, GeneroUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<GeneroDto>("Datos inválidos para actualizar Genero.", "Genero");

            var GeneroExistente = await _GeneroRepo.Obtener(a => a.GeneroId == id, tracked: true);
            if (GeneroExistente == null)
                return ResponseHelper.Fail<GeneroDto>("Genero no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<GeneroDto>(validation.Errors);

            _mapper.Map(updateDto, GeneroExistente);
            await _GeneroRepo.ActualizarGenero(GeneroExistente);

            _logger.LogInformation("✅ Genero ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<GeneroDto>(null, "Genero actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Genero ID {Id}", id);
            return ResponseHelper.FailException<GeneroDto>(ex);
        }
    }

    public async Task<ApiResponse<GeneroDto>> ActualizarParcialGeneroAsync(int id, JsonPatchDocument<GeneroUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<GeneroDto>("Datos inválidos para la actualización parcial.", "Patch");

            var GeneroExistente = await _GeneroRepo.Obtener(a => a.GeneroId == id, tracked: true);
            if (GeneroExistente == null)
                return ResponseHelper.Fail<GeneroDto>("Genero no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<GeneroUpdateDto>(GeneroExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<GeneroDto>(validation.Errors);

            _mapper.Map(dto, GeneroExistente);
            await _GeneroRepo.ActualizarGenero(GeneroExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Genero ID {Id}.", id);
            return ResponseHelper.Success<GeneroDto>(null, "Genero actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Genero ID {Id}", id);
            return ResponseHelper.FailException<GeneroDto>(ex);
        }
    }
}
