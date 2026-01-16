using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaClass.Domain.Dto.Pago;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Domain.Dto.Pago;
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
 * PagoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con los pagos de órdenes.
 * Funcionalidades clave:
 * - Obtener todos los pagos o un pago específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar pagos.
 * - Exportar listado de pagos a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de pagos, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar inconsistencias en la creación o actualización de pagos.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones de pago se realicen correctamente, manteniendo
 * el código limpio, profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class PagoService : IPagoService
{
    private readonly IPagoRepositorio _PagoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PagoService> _logger;
    private readonly IValidator<PagoCreateDto> _createValidator;
    private readonly IValidator<PagoUpdateDto> _updateValidator;
    private readonly IValidator<PagoUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public PagoService(IPagoRepositorio PagoRepo, IMapper mapper, ILogger<PagoService> logger, IValidator<PagoCreateDto> createValidator, IValidator<PagoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PagoUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService
)
    {
        _PagoRepo = PagoRepo;
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

    public async Task<ApiResponse<List<PagoDto>>>  ObtenerTodosLosPagoAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Pagos activos...");

            var Pagos = await _PagoRepo.ObtenerPagosConDetalles();

            if (Pagos == null || !Pagos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Pagos registrados.");
                return ResponseHelper.Fail<List<PagoDto>>(
                    new List<ErrorDetail> { new() { Campo = "Pagos", Mensaje = "No se encontraron Pagos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var PagosDto = _mapper.Map<IEnumerable<PagoDto>>(Pagos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Pagos.", PagosDto.Count);
            return ResponseHelper.Success(PagosDto, "Pagos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Pagos.");
            return ResponseHelper.FailException<List<PagoDto>>(ex);
        }
    }

    public async Task<ApiResponse<PagoDto>> ObtenerPagoPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            var Pago = await _PagoRepo.Obtener(a => a.PagoId == id);
            if (Pago == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Pago con ID {Id}.", id);
                return ResponseHelper.Fail<PagoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Pago con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<PagoDto>(Pago);
            _logger.LogInformation("✅ Pago con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Pago encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Pago por ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelPagosAsync()
    {
        try
        {
            var pagos = await _context.Pagos.ToListAsync();
            var pagosDto = _mapper.Map<List<PagoDto>>(pagos);


            var bytes = await _excelGenericoService.ExportarExcel(
                pagosDto, "Reporte de Pagos", "Pagos"
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

    public async Task<ApiResponse<PagoDto>> CrearPagoAsync(PagoCreateDto dto, int usuarioId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1️⃣ Validación base
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            InfoTarjetas infoTarjeta = null;

            // ================================
            // 2️⃣ MANEJO DE TARJETA
            // ================================
            if (dto.InfoTarjetaId.HasValue && dto.InfoTarjetaId > 0)
            {
                // Tarjeta guardada
                infoTarjeta = await _context.InfomaTarjetas
                    .Include(t => t.DetalleTarjeta)
                    .FirstOrDefaultAsync(t =>
                        t.InfoTarjetaId == dto.InfoTarjetaId &&
                        t.UsuarioId == usuarioId &&
                        t.Estado);

                if (infoTarjeta == null)
                    return ResponseHelper.Fail<PagoDto>("La tarjeta no existe o no pertenece al usuario.");
            }
            else if (dto.NuevaTarjeta != null)
            {
                // Tarjeta nueva
                if (!dto.MedioPagoId.HasValue || dto.MedioPagoId <= 0)
                    return ResponseHelper.Fail<PagoDto>("Debe especificar MedioPagoId al crear una nueva tarjeta.");

                var tarjetaDuplicada = await _context.InfomaTarjetas
                    .Include(t => t.DetalleTarjeta)
                    .AnyAsync(t =>
                        t.UsuarioId == usuarioId &&
                        t.MedioPagoId == dto.MedioPagoId &&
                        t.DetalleTarjeta.NumeroTarjeta == dto.NuevaTarjeta.NumeroTarjeta);

                if (tarjetaDuplicada)
                    return ResponseHelper.Fail<PagoDto>("Esta tarjeta ya está registrada.");

                // Crear detalle tarjeta
                var detalle = new DetalleTarjeta
                {
                    NumeroTarjeta = dto.NuevaTarjeta.NumeroTarjeta,
                    FechaVencimiento = dto.NuevaTarjeta.FechaVencimiento,
                    CVV = dto.NuevaTarjeta.CVV,
                    Estado = true
                };
                _context.DetalleTarjetas.Add(detalle);
                await _context.SaveChangesAsync();

                // Crear info tarjeta
                infoTarjeta = new InfoTarjetas
                {
                    UsuarioId = usuarioId,
                    DetalleTarjetaId = detalle.DetalleTarjetaId,
                    MedioPagoId = dto.MedioPagoId.Value,
                    Estado = true
                };
                _context.InfomaTarjetas.Add(infoTarjeta);
                await _context.SaveChangesAsync();
            }
            else
            {
                return ResponseHelper.Fail<PagoDto>("Debe seleccionar una tarjeta guardada o registrar una nueva.");
            }

            // ================================
            // 3️⃣ CREAR PAGO
            // ================================
            var pago = new Pago
            {
                OrdenId = dto.OrdenId,
                InfoTarjetaId = infoTarjeta.InfoTarjetaId,
                MedioPagoId = infoTarjeta.MedioPagoId,
                CodigoOperacion = dto.CodigoOperacion,
                Estado = dto.Estado,
                FechaRegistro = DateTime.Now
            };
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            // ================================
            // 4️⃣ CONFIRMAR COMPRA Y LIMPIAR CARRITOS ANTIGUOS
            // ================================
            var (exitoCompra, mensajeCompra) = await ConfirmarCompraAsync(dto.OrdenId, usuarioId);

            if (!exitoCompra)
            {
                await transaction.RollbackAsync();
                return ResponseHelper.Fail<PagoDto>($"Pago creado pero no se pudo confirmar la compra: {mensajeCompra}");
            }

            await transaction.CommitAsync();

            var result = _mapper.Map<PagoDto>(pago);
            return ResponseHelper.Success(result, "Pago registrado y stock descontado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            var errorReal = ex.InnerException?.Message ?? ex.Message;
            return ResponseHelper.Fail<PagoDto>(errorReal);
        }
    }

    public async Task<(bool Exito, string Mensaje)> ConfirmarCompraAsync(int ordenId, int usuarioId)
    {
        try
        {
            // 1️⃣ Traer la orden actual con carritos y productos
            var orden = await _context.Ordenes
                .Include(o => o.CarritoCompras)
                    .ThenInclude(c => c.ProductoTalla)
                .FirstOrDefaultAsync(o =>
                    o.OrdenId == ordenId &&
                    o.UsuarioId == usuarioId);

            if (orden == null)
                return (false, "La orden no existe o no pertenece al usuario.");

            if (orden.CarritoCompras == null || !orden.CarritoCompras.Any())
                return (false, "La orden no contiene productos.");

            // 2️⃣ Liberar stock de otros carritos activos del mismo usuario (carritos fantasma)
            var carritosActivosPrevios = await _context.CarritoCompras
                .Include(c => c.ProductoTalla)
                .Where(c => c.UsuarioId == usuarioId
                            && c.Estado // activos
                            && c.OrdenId != ordenId) // excluir la orden actual
                .ToListAsync();

            foreach (var carrito in carritosActivosPrevios)
            {
                if (carrito.ProductoTalla != null)
                {
                    carrito.ProductoTalla.StockReservado -= carrito.Cantidad;
                    if (carrito.ProductoTalla.StockReservado < 0)
                        carrito.ProductoTalla.StockReservado = 0;

                    _context.ProductoTallas.Update(carrito.ProductoTalla);
                }

                carrito.Estado = false; // desactivar carrito fantasma
                _context.CarritoCompras.Update(carrito);
            }

            // 3️⃣ Validar stock disponible para la orden actual
            foreach (var carrito in orden.CarritoCompras)
            {
                var productoTalla = carrito.ProductoTalla;
                if (productoTalla == null)
                    return (false, $"Producto/Talla no encontrada. ProductoTallaId: {carrito.ProductoTallaId}");

                int stockDisponible = productoTalla.Stock - productoTalla.StockReservado;
                if (carrito.Cantidad > stockDisponible)
                    return (false, $"Stock insuficiente para el producto {productoTalla.ProductoId}. Disponible: {stockDisponible}");
            }

            // 4️⃣ Descontar stock de la orden actual y liberar stock reservado
            foreach (var carrito in orden.CarritoCompras)
            {
                var productoTalla = carrito.ProductoTalla;

                productoTalla.Stock -= carrito.Cantidad;
                productoTalla.StockReservado -= carrito.Cantidad;
                if (productoTalla.StockReservado < 0) productoTalla.StockReservado = 0;
                if (productoTalla.Stock <= 0)
                {
                    productoTalla.Stock = 0;
                    productoTalla.Estado = false; // inactivar producto si se agota
                }

                carrito.Estado = false; // carrito comprado
                _context.ProductoTallas.Update(productoTalla);
                _context.CarritoCompras.Update(carrito);
            }

            // 5️⃣ Guardar todos los cambios en una sola operación
            await _context.SaveChangesAsync();

            return (true, "Compra confirmada, stock descontado y carritos previos liberados correctamente.");
        }
        catch (Exception ex)
        {
            var error = ex.InnerException?.Message ?? ex.Message;
            return (false, $"Error al confirmar compra: {error}");
        }
    }



    public async Task<ApiResponse<object>> EliminarPagoAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Pago = await _PagoRepo.Obtener(a => a.PagoId == id);
            if (Pago == null)
                return ResponseHelper.Fail<object>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            await _PagoRepo.Remover(Pago);
            _logger.LogInformation("✅ Pago ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Pago eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Pago ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<PagoDto>> ActualizarPagoAsync(int id, PagoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<PagoDto>("Datos inválidos para actualizar Pago.", "Pago");

            var PagoExistente = await _PagoRepo.Obtener(a => a.PagoId == id, tracked: true);
            if (PagoExistente == null)
                return ResponseHelper.Fail<PagoDto>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            _mapper.Map(updateDto, PagoExistente);
            await _PagoRepo.ActualizarPago(PagoExistente);

            _logger.LogInformation("✅ Pago ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<PagoDto>(null, "Pago actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Pago ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<PagoDto>> ActualizarParcialPagoAsync(int id, JsonPatchDocument<PagoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<PagoDto>("Datos inválidos para la actualización parcial.", "Patch");

            var PagoExistente = await _PagoRepo.Obtener(a => a.PagoId == id, tracked: true);
            if (PagoExistente == null)
                return ResponseHelper.Fail<PagoDto>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<PagoUpdateDto>(PagoExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            _mapper.Map(dto, PagoExistente);
            await _PagoRepo.ActualizarPago(PagoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Pago ID {Id}.", id);
            return ResponseHelper.Success<PagoDto>(null, "Pago actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Pago ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<List<PagoDto>>> ObtenerPagosPorUsuarioAsync(int usuarioId)
    {
        try
        {
            if (usuarioId <= 0)
                return ResponseHelper.Fail<List<PagoDto>>("Usuario no válido.","UsuarioId",HttpStatusCode.BadRequest);

            var pagos = await _PagoRepo.ObtenerPagosPorUsuario(usuarioId);

            if (pagos == null || !pagos.Any())
                return ResponseHelper.Fail<List<PagoDto>>("No se encontraron pagos para el usuario.","Pagos",HttpStatusCode.NotFound);

            var pagosDto = _mapper.Map<List<PagoDto>>(pagos);

            _logger.LogInformation("✅ Se obtuvieron {Count} pagos para el Usuario ID {UsuarioId}.", pagosDto.Count, usuarioId);

            return ResponseHelper.Success<List<PagoDto>>(pagosDto,"Pagos del usuario obtenidos correctamente.",HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"❌ Error al obtener pagos del Usuario ID {UsuarioId}.",usuarioId);

            return ResponseHelper.FailException<List<PagoDto>>(ex);
        }
    }

}



