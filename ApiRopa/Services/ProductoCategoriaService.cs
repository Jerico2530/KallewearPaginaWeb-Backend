using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * ProductoCategoriaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con las categorías de productos.
 * Funcionalidades clave:
 * - Obtener todas las categorías o una específica por ID, incluyendo sus detalles relacionados.
 * - Crear, actualizar (completo o parcial) y eliminar categorías de productos.
 * - Exportar listado de categorías a Excel.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de categorías de productos, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados y mantener consistencia en la actualización de registros.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class ProductoCategoriaService : IProductoCategoriaService
{
    private readonly IProductoCategoriaRepositorio _ProductoCategoriaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoCategoriaService> _logger;
    private readonly IValidator<ProductoCategoriaCreateDto> _createValidator;
    private readonly IValidator<ProductoCategoriaUpdateDto> _updateValidator;
    private readonly IValidator<ProductoCategoriaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;

    public ProductoCategoriaService(IProductoCategoriaRepositorio Producto_CategoriaRepo, IMapper mapper, ILogger<ProductoCategoriaService> logger , IValidator<ProductoCategoriaCreateDto> createValidator, IValidator<ProductoCategoriaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<ProductoCategoriaUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _ProductoCategoriaRepo = Producto_CategoriaRepo;
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

    public async Task<ApiResponse<List<ProductoCategoriaDto>>>  ObtenerProductoCategoriasConDetallesAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los ProductoCategorias activos...");

            var ProductoCategorias = await _ProductoCategoriaRepo.ObtenerProductoCategoriaConDetalles();

            if (ProductoCategorias == null || !ProductoCategorias.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron ProductoCategorias registrados.");
                return ResponseHelper.Fail<List<ProductoCategoriaDto>>(
                    new List<ErrorDetail> { new() { Campo = "ProductoCategorias", Mensaje = "No se encontraron ProductoCategorias registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var ProductoCategoriasDto = _mapper.Map<IEnumerable<ProductoCategoriaDto>>(ProductoCategorias).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} ProductoCategorias.", ProductoCategoriasDto.Count);
            return ResponseHelper.Success(ProductoCategoriasDto, "ProductoCategorias obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener ProductoCategorias.");
            return ResponseHelper.FailException<List<ProductoCategoriaDto>>(ex);
        }
    }

    public async Task<ApiResponse<ProductoCategoriaDto>> ObtenerProductoCategoriaPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoCategoriaDto>(validation.Errors);

            var ProductoCategoria = await _ProductoCategoriaRepo.ObtenerProductoCategoriaConDetallesPorId( id);
            if (ProductoCategoria == null)
            {
                _logger.LogWarning("⚠️ No se encontró el ProductoCategoria con ID {Id}.", id);
                return ResponseHelper.Fail<ProductoCategoriaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el ProductoCategoria con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<ProductoCategoriaDto>(ProductoCategoria);
            _logger.LogInformation("✅ ProductoCategoria con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "ProductoCategoria encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener ProductoCategoria por ID {Id}", id);
            return ResponseHelper.FailException<ProductoCategoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelProductoCategoriasAsync()
    {
        try
        {
            var productoCategorias = await _ProductoCategoriaRepo.ObtenerProductoCategoriaConDetalles();
            var productoCategoriasDto = _mapper.Map<List<ProductoCategoriaDto>>(productoCategorias);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                productoCategoriasDto, "Reporte de ProductoCategorias", "ProductoCategorias", excluir
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


    public async Task<ApiResponse<ProductoCategoriaDto>> CrearProductoCategoriaAsync(ProductoCategoriaCreateDto createDto)
    {

        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<ProductoCategoriaDto>("Datos inválidos para crear ProductoCategoria.", "ProductoCategoria");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoCategoriaDto>(validation.Errors);


            var modelo = _mapper.Map<ProductoCategoria>(createDto);
            await _ProductoCategoriaRepo.Crear(modelo);

            var dto = _mapper.Map<ProductoCategoriaDto>(modelo);
            _logger.LogInformation("✅ ProductoCategoria '{ProductoId}' creado correctamente.", dto.ProductoId);
            return ResponseHelper.Success(dto, "ProductoCategoria creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear ProductoCategoria.");
            return ResponseHelper.FailException<ProductoCategoriaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarProductoCategoriaAsync(int id)
    {

        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var ProductoCategoria = await _ProductoCategoriaRepo.Obtener(a => a.ProductoCategoriaId == id);
            if (ProductoCategoria == null)
                return ResponseHelper.Fail<object>("ProductoCategoria no encontrado.", "Id", HttpStatusCode.NotFound);

            await _ProductoCategoriaRepo.Remover(ProductoCategoria);
            _logger.LogInformation("✅ ProductoCategoria ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "ProductoCategoria eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar ProductoCategoria ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<ProductoCategoriaDto>> ActualizarProductoCategoriaAsync(int id, ProductoCategoriaUpdateDto updateDto)
    {

        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<ProductoCategoriaDto>("Datos inválidos para actualizar ProductoCategoria.", "ProductoCategoria");

            var ProductoCategoriaExistente = await _ProductoCategoriaRepo.Obtener(a => a.ProductoCategoriaId == id, tracked: true);
            if (ProductoCategoriaExistente == null)
                return ResponseHelper.Fail<ProductoCategoriaDto>("ProductoCategoria no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoCategoriaDto>(validation.Errors);

            _mapper.Map(updateDto, ProductoCategoriaExistente);
            await _ProductoCategoriaRepo.ActualizarProductoCategoria(ProductoCategoriaExistente);

            _logger.LogInformation("✅ ProductoCategoria ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<ProductoCategoriaDto>(null, "ProductoCategoria actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar ProductoCategoria ID {Id}", id);
            return ResponseHelper.FailException<ProductoCategoriaDto>(ex);
        }
    }

}





