using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Pregunta;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * PreguntaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con preguntas.
 * Funcionalidades clave:
 * - Obtener todas las preguntas o una específica por ID.
 * - Crear, actualizar (completo o parcial) y eliminar preguntas.
 * - Exportar listado de preguntas a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de preguntas, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados y asegurar coherencia en las actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando operaciones correctas y manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class PreguntaService : IPreguntaService
{
    private readonly IPreguntaRepositorio _PreguntaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PreguntaService> _logger;
    private readonly IValidator<PreguntaCreateDto> _createValidator;
    private readonly IValidator<PreguntaUpdateDto> _updateValidator;
    private readonly IValidator<PreguntaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public PreguntaService(IPreguntaRepositorio PreguntaRepo, IMapper mapper, ILogger<PreguntaService> logger , IValidator<PreguntaCreateDto> createValidator, IValidator<PreguntaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PreguntaUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _PreguntaRepo = PreguntaRepo;
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

    public async Task<ApiResponse<List<PreguntaDto>>> ObtenerTodosLosPreguntaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Preguntas activos...");

            var Preguntas = await _PreguntaRepo.ObtenerTodo();

            if (Preguntas == null || !Preguntas.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Preguntas registrados.");
                return ResponseHelper.Fail<List<PreguntaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Preguntas", Mensaje = "No se encontraron Preguntas registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var PreguntasDto = _mapper.Map<IEnumerable<PreguntaDto>>(Preguntas).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Preguntas.", PreguntasDto.Count);
            return ResponseHelper.Success(PreguntasDto, "Preguntas obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Preguntas.");
            return ResponseHelper.FailException<List<PreguntaDto>>(ex);
        }
    }

    public async Task<ApiResponse<PreguntaDto>> ObtenerPreguntaPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PreguntaDto>(validation.Errors);

            var Pregunta = await _PreguntaRepo.Obtener(a => a.PreguntaId == id);
            if (Pregunta == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Pregunta con ID {Id}.", id);
                return ResponseHelper.Fail<PreguntaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Pregunta con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<PreguntaDto>(Pregunta);
            _logger.LogInformation("✅ Pregunta con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Pregunta encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Pregunta por ID {Id}", id);
            return ResponseHelper.FailException<PreguntaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelPreguntasAsync()
    {
        try
        {
            var preguntas = await _context.Preguntas.ToListAsync();
            var preguntasDto = _mapper.Map<List<PreguntaDto>>(preguntas);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                preguntasDto, "Reporte de Preguntas", "Preguntas", excluir
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


    public async Task<ApiResponse<PreguntaDto>> CrearPreguntaAsync(PreguntaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<PreguntaDto>("Datos inválidos para crear Pregunta.", "Pregunta");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PreguntaDto>(validation.Errors);

            var modelo = _mapper.Map<Pregunta>(createDto);
            await _PreguntaRepo.Crear(modelo);

            var dto = _mapper.Map<PreguntaDto>(modelo);
            _logger.LogInformation("✅ Pregunta '{Preguntas}' creado correctamente.", dto.Preguntas);
            return ResponseHelper.Success(dto, "Pregunta creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Pregunta.");
            return ResponseHelper.FailException<PreguntaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarPreguntaAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Pregunta = await _PreguntaRepo.Obtener(a => a.PreguntaId == id);
            if (Pregunta == null)
                return ResponseHelper.Fail<object>("Pregunta no encontrado.", "Id", HttpStatusCode.NotFound);

            await _PreguntaRepo.Remover(Pregunta);
            _logger.LogInformation("✅ Pregunta ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Pregunta eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Pregunta ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<PreguntaDto>> ActualizarPreguntaAsync(int id, PreguntaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<PreguntaDto>("Datos inválidos para actualizar Pregunta.", "Pregunta");

            var PreguntaExistente = await _PreguntaRepo.Obtener(a => a.PreguntaId == id, tracked: true);
            if (PreguntaExistente == null)
                return ResponseHelper.Fail<PreguntaDto>("Pregunta no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PreguntaDto>(validation.Errors);

            _mapper.Map(updateDto, PreguntaExistente);
            await _PreguntaRepo.ActualizarPregunta(PreguntaExistente);

            _logger.LogInformation("✅ Pregunta ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<PreguntaDto>(null, "Pregunta actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Pregunta ID {Id}", id);
            return ResponseHelper.FailException<PreguntaDto>(ex);
        }
    }

    public async Task<ApiResponse<PreguntaDto>> ActualizarParcialPreguntaAsync(int id, JsonPatchDocument<PreguntaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<PreguntaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var PreguntaExistente = await _PreguntaRepo.Obtener(a => a.PreguntaId == id, tracked: true);
            if (PreguntaExistente == null)
                return ResponseHelper.Fail<PreguntaDto>("Pregunta no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<PreguntaUpdateDto>(PreguntaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PreguntaDto>(validation.Errors);

            _mapper.Map(dto, PreguntaExistente);
            await _PreguntaRepo.ActualizarPregunta(PreguntaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Pregunta ID {Id}.", id);
            return ResponseHelper.Success<PreguntaDto>(null, "Pregunta actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Pregunta ID {Id}", id);
            return ResponseHelper.FailException<PreguntaDto>(ex);
        }
    }
}
