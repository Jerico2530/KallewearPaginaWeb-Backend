using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Noticia;
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
 * NoticiaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con noticias.
 * Funcionalidades clave:
 * - Obtener todas las noticias o una específica por ID.
 * - Crear, actualizar (completo o parcial) y eliminar noticias.
 * - Exportar listado de noticias a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de noticias, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados en creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class NoticiaService : INoticiaService
{
    private readonly INoticiaRepositorio _NoticiaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<NoticiaService> _logger;
    private readonly IValidator<NoticiaCreateDto> _createValidator;
    private readonly IValidator<NoticiaUpdateDto> _updateValidator;
    private readonly IValidator<NoticiaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public NoticiaService(INoticiaRepositorio NoticiaRepo, IMapper mapper, ILogger<NoticiaService> logger , IValidator<NoticiaCreateDto> createValidator, IValidator<NoticiaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<NoticiaUpdateDto> patchValidator,
AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _NoticiaRepo = NoticiaRepo;
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

    public async Task<ApiResponse<List<NoticiaDto>>> ObtenerTodosLosNoticiaAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Noticias activos...");

            var Noticias = await _NoticiaRepo.ObtenerTodo();

            if (Noticias == null || !Noticias.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Noticias registrados.");
                return ResponseHelper.Fail<List<NoticiaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Noticias", Mensaje = "No se encontraron Noticias registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var NoticiasDto = _mapper.Map<IEnumerable<NoticiaDto>>(Noticias).OrderBy(a => a.Titulo).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Noticias.", NoticiasDto.Count);
            return ResponseHelper.Success(NoticiasDto, "Noticias obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Noticias.");
            return ResponseHelper.FailException<List<NoticiaDto>>(ex);
        }
    }

    public async Task<ApiResponse<NoticiaDto>> ObtenerNoticiaPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<NoticiaDto>(validation.Errors);

            var Noticia = await _NoticiaRepo.Obtener(a => a.NoticiaId == id);
            if (Noticia == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Noticia con ID {Id}.", id);
                return ResponseHelper.Fail<NoticiaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Noticia con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<NoticiaDto>(Noticia);
            _logger.LogInformation("✅ Noticia con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Noticia encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Noticia por ID {Id}", id);
            return ResponseHelper.FailException<NoticiaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelNoticiasAsync()
    {
        try
        {
            var noticias = await _context.Noticias.ToListAsync();
            var noticiasDto = _mapper.Map<List<NoticiaDto>>(noticias);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                noticiasDto, "Reporte de Noticias", "Noticias", excluir
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



    public async Task<ApiResponse<NoticiaDto>> CrearNoticiaAsync(NoticiaCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<NoticiaDto>("Datos inválidos para crear Noticia.", "Noticia");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<NoticiaDto>(validation.Errors);

            var existe = await _NoticiaRepo.Obtener(a => a.Titulo.ToLower() == createDto.Titulo.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<NoticiaDto>("Ya existe un Noticia con ese título.", "Titulo", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Noticia>(createDto);
            await _NoticiaRepo.Crear(modelo);

            var dto = _mapper.Map<NoticiaDto>(modelo);
            _logger.LogInformation("✅ Noticia '{Titulo}' creado correctamente.", dto.Titulo);
            return ResponseHelper.Success(dto, "Noticia creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Noticia.");
            return ResponseHelper.FailException<NoticiaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarNoticiaAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Noticia = await _NoticiaRepo.Obtener(a => a.NoticiaId == id);
            if (Noticia == null)
                return ResponseHelper.Fail<object>("Noticia no encontrado.", "Id", HttpStatusCode.NotFound);

            await _NoticiaRepo.Remover(Noticia);
            _logger.LogInformation("✅ Noticia ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Noticia eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Noticia ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<NoticiaDto>> ActualizarNoticiaAsync(int id, NoticiaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<NoticiaDto>("Datos inválidos para actualizar Noticia.", "Noticia");

            var NoticiaExistente = await _NoticiaRepo.Obtener(a => a.NoticiaId == id, tracked: true);
            if (NoticiaExistente == null)
                return ResponseHelper.Fail<NoticiaDto>("Noticia no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<NoticiaDto>(validation.Errors);

            _mapper.Map(updateDto, NoticiaExistente);
            await _NoticiaRepo.ActualizarNoticia(NoticiaExistente);

            _logger.LogInformation("✅ Noticia ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<NoticiaDto>(null, "Noticia actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Noticia ID {Id}", id);
            return ResponseHelper.FailException<NoticiaDto>(ex);
        }
    }

    public async Task<ApiResponse<NoticiaDto>> ActualizarParcialNoticiaAsync(int id, JsonPatchDocument<NoticiaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<NoticiaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var NoticiaExistente = await _NoticiaRepo.Obtener(a => a.NoticiaId == id, tracked: true);
            if (NoticiaExistente == null)
                return ResponseHelper.Fail<NoticiaDto>("Noticia no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<NoticiaUpdateDto>(NoticiaExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<NoticiaDto>(validation.Errors);

            _mapper.Map(dto, NoticiaExistente);
            await _NoticiaRepo.ActualizarNoticia(NoticiaExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Noticia ID {Id}.", id);
            return ResponseHelper.Success<NoticiaDto>(null, "Noticia actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Noticia ID {Id}", id);
            return ResponseHelper.FailException<NoticiaDto>(ex);
        }
    }
}

