using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Categoria;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * CategoriaService
 *
 * Servicio de dominio encargado de gestionar la lógica principal de las categorías.
 * Funcionalidades clave:
 * - Obtener todas las categorías o una por ID.
 * - Crear, actualizar (completo o parcial) y eliminar categorías.
 * - Exportar listado de categorías a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de las categorías y garantizar integridad en:
 * - Existencia y unicidad de títulos
 * - Validación de datos
 * - Operaciones de CRUD
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepositorio _categoriaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriaService> _logger;
    private readonly IValidator<CategoriaCreateDto> _createValidator;
    private readonly IValidator<CategoriaUpdateDto> _updateValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public CategoriaService(ICategoriaRepositorio CategoriaRepo, IMapper mapper, ILogger<CategoriaService> logger , IValidator<CategoriaCreateDto> createValidator, IValidator<CategoriaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator,AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _categoriaRepo = CategoriaRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _context = context;
        _excelGenericoService = excelGenericoService;

    }

    public async Task<ApiResponse<List<CategoriaDto>>> ObtenerTodosLosCategoriasAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los categorias activos...");

            var Categorias = await _categoriaRepo.ObtenerTodo();

            if (Categorias == null || !Categorias.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron categorias registrados.");
                return ResponseHelper.Fail<List<CategoriaDto>>(
                    new List<ErrorDetail> { new() { Campo = "Categoria", Mensaje = "No se encontraron categorias registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var CategoriasDto = _mapper.Map<IEnumerable<CategoriaDto>>(Categorias).ToList();

            _logger.LogInformation("✅ {Count} categorias obtenidos exitosamente.", CategoriasDto.Count());
            return ResponseHelper.Success(CategoriasDto, "Categorias obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener categorias.");
            return ResponseHelper.FailException<List<CategoriaDto>>(ex);
        }
    }

    public async Task<ApiResponse<CategoriaDto>> ObtenerCategoriaPorIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("🔍 Buscando categorias con ID {Id}", id);
            // Validación de ID
            ValidationResult validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CategoriaDto>(validation.Errors);

            var Categoria = await _categoriaRepo.Obtener(p => p.CategoriaId == id);

            if (Categoria == null)
            {
                _logger.LogWarning("⚠️ Categoria con ID {Id} no encontrado.", id);
                return ResponseHelper.Fail<CategoriaDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Categoria con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
            }

            var dto = _mapper.Map<CategoriaDto>(Categoria);
            _logger.LogInformation("✅ Categoria ID {Id} obtenido exitosamente.", id);
            return ResponseHelper.Success(dto, "Categoria encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener categoria con ID {Id}", id);
            return ResponseHelper.FailException<CategoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelCategoriasAsync()
    {
        try
        {
            var categorias = await _context.Categorias.ToListAsync();
            var categoriasDto = _mapper.Map<List<CategoriaDto>>(categorias);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                categoriasDto, "Reporte de Categorias", "Categorias", excluir
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


    public async Task<ApiResponse<CategoriaDto>> CrearCategoriaAsync(CategoriaCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("🟢 Iniciando creación de categoria: {DesCategoria}", createDto?.DesCategoria);

            if (createDto == null)
                return ResponseHelper.Fail<CategoriaDto>("Datos inválidos para crear Categoria.", "Categoria");

            //  Validación de DTO
            ValidationResult validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CategoriaDto>(validation.Errors);

            // Validación de título duplicado
            var existe = await _categoriaRepo.Obtener(p => p.DesCategoria.ToLower() == createDto.DesCategoria.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<CategoriaDto>("Ya existe un Categoria con ese título.", "Titulo", HttpStatusCode.Conflict);

            var modelo = _mapper.Map<Categoria>(createDto);
            await _categoriaRepo.Crear(modelo);
            _logger.LogInformation("✅ Categoria '{Categoria}' guardado correctamente en la base de datos con ID {Id}", createDto.DesCategoria, modelo.CategoriaId);


            var dto = _mapper.Map<CategoriaDto>(modelo);
            _logger.LogInformation("✅ Categoria '{Titulo}' creado correctamente.", dto.DesCategoria);
            return ResponseHelper.Success(dto, "Categoria creado correctamente", HttpStatusCode.Created);
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear categoria {DesCategoria}: {Error}", createDto?.DesCategoria, ex.Message);
            return ResponseHelper.FailException<CategoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarCategoriaAsync(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Eliminando categoria ID {Id}", id);

            ValidationResult validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Categoria = await _categoriaRepo.Obtener(p => p.CategoriaId == id);
            if (Categoria == null)
                return ResponseHelper.Fail<object>("Categoria no encontrado.", "Id", HttpStatusCode.NotFound);

            await _categoriaRepo.Remover(Categoria);
            _logger.LogInformation("✅ Categoria ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Categoria eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar categoria ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<CategoriaDto>> ActualizarCategoriaAsync(int id, CategoriaUpdateDto updateDto)
    {
        try
        {
            _logger.LogInformation("✏️ Iniciando actualización completa del categoria ID {Id}", id);

            if (updateDto == null)
                return ResponseHelper.Fail<CategoriaDto>("Datos inválidos para actualizar Categoria.", "Categoria");

            // Obtener la entidad existente
            var testimonioExistente = await _categoriaRepo.Obtener(p => p.CategoriaId == id, tracked: true);
            if (testimonioExistente == null)
                return ResponseHelper.Fail<CategoriaDto>("Categoria no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CategoriaDto>(validation.Errors);

            _mapper.Map(updateDto, testimonioExistente);

            await _categoriaRepo.ActualizarCategoria(testimonioExistente);

            _logger.LogInformation("✅ Categoria ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<CategoriaDto>(null, "Categoria actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar categoria ID {Id}", id);
            return ResponseHelper.FailException<CategoriaDto>(ex);
        }
    }


    public async Task<ApiResponse<CategoriaDto>> ActualizarParcialCategoriaAsync(int id, JsonPatchDocument<CategoriaUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<CategoriaDto>("Datos inválidos para la actualización parcial.", "Patch");

            var Categoria = await _categoriaRepo.Obtener(v => v.CategoriaId == id, tracked: false);

            if (Categoria == null)
                return ResponseHelper.Fail<CategoriaDto>("Categoria no encontrado.", "Id", HttpStatusCode.NotFound);

            var CategoriaDto = _mapper.Map<CategoriaUpdateDto>(Categoria);
            patchDto.ApplyTo(CategoriaDto);

            ValidationResult validation = await _updateValidator.ValidateAsync(CategoriaDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CategoriaDto>(validation.Errors);

            _mapper.Map(CategoriaDto, Categoria);
            await _categoriaRepo.ActualizarCategoria(Categoria);

            _logger.LogInformation("✅ PATCH aplicado correctamente al categoria ID {Id}.", id);
            return ResponseHelper.Success<CategoriaDto>(null, "Categoria actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al categoria ID {Id}", id);

            return ResponseHelper.FailException<CategoriaDto>(ex);
        }
    }
}

