using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using AutoMapper;
using Azure;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Producto;
using BiblotecaWeb.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
/*
 * ProductoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con productos.
 * Funcionalidades clave:
 * - Obtener todos los productos o uno específico por ID, incluyendo sus detalles asociados.
 * - Crear, actualizar (completo o parcial) y eliminar productos.
 * - Exportar listado de productos a Excel.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de productos, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados y mantener consistencia en la actualización de registros.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepositorio _ProductoRepo;
        private readonly IMapper _mapper;
        private readonly IValidator<ProductoCreateDto> _createValidator;
        private readonly IValidator<ProductoUpdateDto> _updateValidator;
        private readonly IValidator<ProductoUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;
        private readonly ILogger<ProductoService> _logger;
        private readonly AppDbContext _context;
        private readonly ExcelGenericoService _excelGenericoService;



        public ProductoService(IProductoRepositorio productoRepo, IMapper mapper, ILogger<ProductoService> logger , IValidator<ProductoCreateDto> createValidator, IValidator<ProductoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<ProductoUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
        {
            _ProductoRepo = productoRepo;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _getValidator = getValidator;
            _deleteValidator = deleteValidator;
            _patchValidator = patchValidator;
            _logger = logger;
            _context = context;
            _excelGenericoService = excelGenericoService;

        }

        public async Task<ApiResponse<List<ProductoDto>>> ObtenerTodosLosProductosAsync()
        {

            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los Productos activos...");

                var Productos = await _ProductoRepo.ObtenerProductosConDetalles();

                if (Productos == null || !Productos.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron Productos registrados.");
                    return ResponseHelper.Fail<List<ProductoDto>>(
                        new List<ErrorDetail> { new() { Campo = "Productos", Mensaje = "No se encontraron Productos registrados." } },
                        HttpStatusCode.NotFound
                    );
                }

                var ProductosDto = _mapper.Map<IEnumerable<ProductoDto>>(Productos).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} Productos.", ProductosDto.Count);
                return ResponseHelper.Success(ProductosDto, "Productos obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Productos.");
                return ResponseHelper.FailException<List<ProductoDto>>(ex);
            }
        }

        public async Task<ApiResponse<ProductoDto>> ObtenerProductoPorIdAsync(int id)
        {

            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                var Producto = await _ProductoRepo.ObtenerProductoConDetallesPorId(id);
                if (Producto == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Producto con ID {Id}.", id);
                    return ResponseHelper.Fail<ProductoDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Producto con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<ProductoDto>(Producto);
                _logger.LogInformation("✅ Producto con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Producto encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Producto por ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }

        public async Task<ApiResponse<byte[]>> ExportarExcelProductosAsync()
        {
            try
            {
                var productos = await _ProductoRepo.ObtenerProductosConDetalles();
                var productosDto = _mapper.Map<List<ProductoDto>>(productos);

                // Excluir propiedades sensibles o imágenes
                var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

                var bytes = await _excelGenericoService.ExportarExcel(
                    productosDto, "Reporte de Productos", "Productos", excluir
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


        public async Task<ApiResponse<ProductoDto>> CrearProductoAsync(ProductoCreateDto createDto)
        {

            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<ProductoDto>("Datos inválidos para crear Producto.", "Producto");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                var modelo = _mapper.Map<Producto>(createDto);
                await _ProductoRepo.Crear(modelo);

                var dto = _mapper.Map<ProductoDto>(modelo);
                _logger.LogInformation("✅ Producto '{Nombre}' creado correctamente.", dto.Nombre);
                return ResponseHelper.Success(dto, "Producto creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear Producto.");
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarProductoAsync(int id)
        {

            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var Producto = await _ProductoRepo.Obtener(a => a.ProductoId == id);
                if (Producto == null)
                    return ResponseHelper.Fail<object>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                await _ProductoRepo.Remover(Producto);
                _logger.LogInformation("✅ Producto ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Producto eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Producto ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }


        public async Task<ApiResponse<ProductoDto>> ActualizarProductoAsync(int id, ProductoUpdateDto updateDto)
        {

            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<ProductoDto>("Datos inválidos para actualizar Producto.", "Producto");

                var ProductoExistente = await _ProductoRepo.Obtener(a => a.ProductoId == id, tracked: true);
                if (ProductoExistente == null)
                    return ResponseHelper.Fail<ProductoDto>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                _mapper.Map(updateDto, ProductoExistente);
                await _ProductoRepo.ActualizarProducto(ProductoExistente);

                _logger.LogInformation("✅ Producto ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<ProductoDto>(null, "Producto actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar Producto ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }

        public async Task<ApiResponse<ProductoDto>> ActualizarParcialProductoAsync(int id, JsonPatchDocument<ProductoUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<ProductoDto>("Datos inválidos para la actualización parcial.", "Patch");

                var ProductoExistente = await _ProductoRepo.Obtener(a => a.ProductoId == id, tracked: true);
                if (ProductoExistente == null)
                    return ResponseHelper.Fail<ProductoDto>("Producto no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<ProductoUpdateDto>(ProductoExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<ProductoDto>(validation.Errors);

                _mapper.Map(dto, ProductoExistente);
                await _ProductoRepo.ActualizarProducto(ProductoExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Producto ID {Id}.", id);
                return ResponseHelper.Success<ProductoDto>(null, "Producto actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Producto ID {Id}", id);
                return ResponseHelper.FailException<ProductoDto>(ex);
            }
        }
    }
}
