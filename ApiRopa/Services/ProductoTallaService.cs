using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.ProductoTalla;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * ProductoTallaService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con las tallas de productos.
 * Funcionalidades clave:
 * - Obtener todas las tallas de productos o una específica por ID, incluyendo detalles relacionados.
 * - Crear, actualizar (completo o parcial) y eliminar tallas de productos.
 * - Exportar listado de tallas de productos a Excel.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de tallas de productos, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados y mantener consistencia en la actualización de registros.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class ProductoTallaService : IProductoTallaService
{
    private readonly IProductoTallaRepositorio _ProductoTallaRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductoTallaService> _logger;
    private readonly IValidator<ProductoTallaCreateDto> _createValidator;
    private readonly IValidator<ProductoTallaUpdateDto> _updateValidator;
    private readonly IValidator<ProductoTallaUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;



    public ProductoTallaService(IProductoTallaRepositorio ProductoTallaRepo, IMapper mapper, ILogger<ProductoTallaService> logger, IValidator<ProductoTallaCreateDto> createValidator, IValidator<ProductoTallaUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<ProductoTallaUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _ProductoTallaRepo = ProductoTallaRepo;
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

    public async Task<ApiResponse<List<ProductoTallaDto>>> ObtenerProductoTallaConDetallesAsync()
    {

        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los ProductoTallaos activos...");

            var ProductoTallaos = await _ProductoTallaRepo.ObtenerProductoTallasConDetalles();

            if (ProductoTallaos == null || !ProductoTallaos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron ProductoTallaos registrados.");
                return ResponseHelper.Fail<List<ProductoTallaDto>>(
                    new List<ErrorDetail> { new() { Campo = "ProductoTallaos", Mensaje = "No se encontraron ProductoTallaos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var ProductoTallaosDto = _mapper.Map<IEnumerable<ProductoTallaDto>>(ProductoTallaos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} ProductoTallaos.", ProductoTallaosDto.Count);
            return ResponseHelper.Success(ProductoTallaosDto, "ProductoTallaos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener ProductoTallaos.");
            return ResponseHelper.FailException<List<ProductoTallaDto>>(ex);
        }
    }

    public async Task<ApiResponse<ProductoTallaDto>> ObtenerProductoTallaPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoTallaDto>(validation.Errors);

            var ProductoTallao = await _ProductoTallaRepo.ObtenerProductoTallaConDetallesPorId(id);
            if (ProductoTallao == null)
            {
                _logger.LogWarning("⚠️ No se encontró el ProductoTallao con ID {Id}.", id);
                return ResponseHelper.Fail<ProductoTallaDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el ProductoTallao con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<ProductoTallaDto>(ProductoTallao);
            _logger.LogInformation("✅ ProductoTallao con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "ProductoTallao encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener ProductoTallao por ID {Id}", id);
            return ResponseHelper.FailException<ProductoTallaDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelProductoTallasAsync()
    {
        try
        {
            var productoTallas = await _ProductoTallaRepo.ObtenerProductoTallasConDetalles();
            var productoTallasDto = _mapper.Map<List<ProductoTallaDto>>(productoTallas);



            var bytes = await _excelGenericoService.ExportarExcel(
                productoTallasDto, "Reporte de ProductoTallas", "ProductoTallas"
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




    public async Task<ApiResponse<ProductoTallaDto>> CrearProductoTallaAsync(ProductoTallaCreateDto createDto)
    {

        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<ProductoTallaDto>("Datos inválidos para crear ProductoTallao.", "ProductoTallao");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoTallaDto>(validation.Errors);


            var modelo = _mapper.Map<ProductoTalla>(createDto);
            await _ProductoTallaRepo.Crear(modelo);

            var dto = _mapper.Map<ProductoTallaDto>(modelo);
            _logger.LogInformation("✅ ProductoTallao '{ProductoId}' creado correctamente.", dto.ProductoId);
            return ResponseHelper.Success(dto, "ProductoTallao creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear ProductoTallao.");
            return ResponseHelper.FailException<ProductoTallaDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarProductoTallaAsync(int id)
    {

        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var ProductoTallao = await _ProductoTallaRepo.Obtener(a => a.ProductoTallaId == id);
            if (ProductoTallao == null)
                return ResponseHelper.Fail<object>("ProductoTallao no encontrado.", "Id", HttpStatusCode.NotFound);

            await _ProductoTallaRepo.Remover(ProductoTallao);
            _logger.LogInformation("✅ ProductoTallao ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "ProductoTallao eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar ProductoTallao ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<ProductoTallaDto>> ActualizarProductoTallaAsync(int id, ProductoTallaUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<ProductoTallaDto>("Datos inválidos para actualizar ProductoTallao.", "ProductoTallao");

            var ProductoTallaoExistente = await _ProductoTallaRepo.Obtener(a => a.ProductoTallaId == id, tracked: true);
            if (ProductoTallaoExistente == null)
                return ResponseHelper.Fail<ProductoTallaDto>("ProductoTallao no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<ProductoTallaDto>(validation.Errors);

            _mapper.Map(updateDto, ProductoTallaoExistente);
            await _ProductoTallaRepo.ActualizarProductoTalla(ProductoTallaoExistente);

            _logger.LogInformation("✅ ProductoTallao ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<ProductoTallaDto>(null, "ProductoTallao actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar ProductoTallao ID {Id}", id);
            return ResponseHelper.FailException<ProductoTallaDto>(ex);
        }
    }

}
