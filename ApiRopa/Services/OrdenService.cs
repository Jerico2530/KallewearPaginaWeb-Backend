using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Orden;
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
 * OrdenService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con órdenes de compra.
 * Funcionalidades clave:
 * - Obtener todas las órdenes o una por ID.
 * - Crear, actualizar (completo o parcial) y eliminar órdenes.
 * - Exportar listado de órdenes a Excel.
 * - Validar datos de entrada mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de las órdenes, asegurando consistencia y seguridad:
 * - Validación de información de entrada y reglas de negocio.
 * - Cálculo seguro de totales y subtotales.
 * - Evitar inconsistencias en la creación y actualización de órdenes.
 *
 * Este servicio actúa como capa intermedia entre los controladores y los repositorios,
 * garantizando que las operaciones sean correctas, eficientes y mantenibles.
 */
namespace ApiRopa;

public class OrdenService : IOrdenService
{
    private readonly IOrdenRepositorio _OrdenRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdenService> _logger;
    private readonly IValidator<OrdenCreateDto> _createValidator;
    private readonly IValidator<OrdenUpdateDto> _updateValidator;
    private readonly IValidator<OrdenUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;



    public OrdenService(IOrdenRepositorio OrdenRepo, IMapper mapper, ILogger<OrdenService> logger , IValidator<OrdenCreateDto> createValidator, IValidator<OrdenUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<OrdenUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService
)
    {
        _OrdenRepo = OrdenRepo;
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

    public async Task<ApiResponse<List<OrdenDto>>> ObtenerTodosLosOrdenAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Ordens activos...");

            var Ordens = await _OrdenRepo.ObtenerCarritoCompraConDetalles();

            if (Ordens == null || !Ordens.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Ordens registrados.");
                return ResponseHelper.Fail<List<OrdenDto>>(
                    new List<ErrorDetail> { new() { Campo = "Ordens", Mensaje = "No se encontraron Ordens registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var OrdensDto = _mapper.Map<IEnumerable<OrdenDto>>(Ordens).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Ordens.", OrdensDto.Count);
            return ResponseHelper.Success(OrdensDto, "Ordens obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Ordens.");
            return ResponseHelper.FailException<List<OrdenDto>>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDto>> ObtenerOrdenPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDto>(validation.Errors);

            var Orden = await _OrdenRepo.ObtenerOrdenConDetallesPorIdAsync(id);
            if (Orden == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Orden con ID {Id}.", id);
                return ResponseHelper.Fail<OrdenDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Orden con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<OrdenDto>(Orden);
            _logger.LogInformation("✅ Orden con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Orden encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Orden por ID {Id}", id);
            return ResponseHelper.FailException<OrdenDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelOrdenesAsync()
    {
        try
        {
            var ordenes = await _OrdenRepo.ObtenerCarritoCompraConDetalles();
            var ordenesDto = _mapper.Map<List<OrdenDto>>(ordenes);


            var bytes = await _excelGenericoService.ExportarExcel(
                ordenesDto, "Reporte de Usuarios", "Usuarios"
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


    public async Task<ApiResponse<OrdenDto>> CrearOrdenAsync(OrdenCreateDto createDto)
    {
        try
        {

            if (createDto == null)
                return ResponseHelper.Fail<OrdenDto>("Datos inválidos para crear Orden.", "Orden");


            // 🔍 Validación de DTO
            ValidationResult validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDto>(validation.Errors);

            if (string.IsNullOrWhiteSpace(createDto.MetodoEntrega))
                return ResponseHelper.Fail<OrdenDto>("El método de entrega es obligatorio.", "MetodoEntrega");

            var metodosValidos = new[] { "RetiroTienda", "Envio" };
            if (!metodosValidos.Contains(createDto.MetodoEntrega))
                return ResponseHelper.Fail<OrdenDto>(
                    $"El método de entrega '{createDto.MetodoEntrega}' no está disponible. " +
                    $"Opciones válidas: {string.Join(", ", metodosValidos)}",
                    "MetodoEntrega");

            if (createDto.MetodoEntrega == "Envio" && createDto.DireccionId == null)
                return ResponseHelper.Fail<OrdenDto>("Debe seleccionar una dirección para envío.", "DireccionId");

            if (createDto.MetodoEntrega == "RetiroTienda" && createDto.SucursalId == null)
                return ResponseHelper.Fail<OrdenDto>("Debe seleccionar una sucursal para retiro.", "SucursalId");

            // 🔹 Obtener items del carrito que no tienen orden
            var carritos = await _OrdenRepo.ObtenerCarritoSinOrden(createDto.UsuarioId);

            if (carritos == null || !carritos.Any())
                return ResponseHelper.Fail<OrdenDto>("No hay productos en el carrito para crear la orden.", "Carrito");


            // 🔹 Recalcular subtotales en backend (seguridad + consistencia)
            foreach (var item in carritos)
            {
                item.SubTotal = item.Cantidad * item.PrecioUnitario;
            }

            // 🔹 Calcular total de la orden
            createDto.Total = carritos.Sum(c => c.SubTotal);

            // 🔹 Mapear DTO a modelo de dominio
            Orden nuevaOrden = _mapper.Map<Orden>(createDto);

            // 🔹 Guardar la orden
            await _OrdenRepo.Crear(nuevaOrden);

            // 🔹 Asignar OrdenId a los items del carrito y actualizar
            foreach (var item in carritos)
            {
                item.OrdenId = nuevaOrden.OrdenId;
            }
            await _OrdenRepo.ActualizarCarritos(carritos);

            var ordenDto = _mapper.Map<OrdenDto>(nuevaOrden);

            _logger.LogInformation("✅ Orden creada exitosamente: {@Orden}", nuevaOrden);
            return ResponseHelper.Success(ordenDto, "Orden creada exitosamente.", HttpStatusCode.Created);
        }
        
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear orden {Usuario}: {Error}", createDto?.UsuarioId, ex.Message);
            return ResponseHelper.Fail<OrdenDto>($"Error interno al crear la orden: {ex.Message}",code: HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ApiResponse<object>> EliminarOrdenAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Orden = await _OrdenRepo.Obtener(a => a.OrdenId == id);
            if (Orden == null)
                return ResponseHelper.Fail<object>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

            await _OrdenRepo.Remover(Orden);
            _logger.LogInformation("✅ Orden ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Orden eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Orden ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDto>> ActualizarOrdenAsync(int id, OrdenUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<OrdenDto>("Datos inválidos para actualizar Orden.", "Orden");

            var OrdenExistente = await _OrdenRepo.Obtener(a => a.OrdenId == id, tracked: true);
            if (OrdenExistente == null)
                return ResponseHelper.Fail<OrdenDto>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDto>(validation.Errors);

            _mapper.Map(updateDto, OrdenExistente);
            await _OrdenRepo.ActualizarOrden(OrdenExistente);

            _logger.LogInformation("✅ Orden ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<OrdenDto>(null, "Orden actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Orden ID {Id}", id);
            return ResponseHelper.FailException<OrdenDto>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDto>> ActualizarParcialOrdenAsync(int id, JsonPatchDocument<OrdenUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<OrdenDto>("Datos inválidos para la actualización parcial.", "Patch");

            var OrdenExistente = await _OrdenRepo.Obtener(a => a.OrdenId == id, tracked: true);
            if (OrdenExistente == null)
                return ResponseHelper.Fail<OrdenDto>("Orden no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<OrdenUpdateDto>(OrdenExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDto>(validation.Errors);

            _mapper.Map(dto, OrdenExistente);
            await _OrdenRepo.ActualizarOrden(OrdenExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Orden ID {Id}.", id);
            return ResponseHelper.Success<OrdenDto>(null, "Orden actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Orden ID {Id}", id);
            return ResponseHelper.FailException<OrdenDto>(ex);
        }
    }
}

