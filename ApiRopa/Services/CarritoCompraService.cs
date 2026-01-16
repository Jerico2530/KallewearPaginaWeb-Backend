using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Services;
using ApiRopa.Services.Dominio;
using ApiRopa.Services.Help;
using ApiRopa.Services.IServices;
using AutoMapper;
using Azure;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * CarritoCompraService
 *
 * Servicio de dominio encargado de gestionar la lógica principal del carrito de compras.
 * Funcionalidades clave:
 * - Obtener carrito(s) con detalles y cálculo de totales.
 * - Agregar, actualizar y eliminar items del carrito.
 * - Manejar stock: reservar, liberar y confirmar antes de una compra.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y lógica de dominio.
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio del carrito y garantizar integridad en:
 * - Cantidades
 * - Stock disponible
 * - Totales calculados
 * - Estados de los ítems
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class CarritoCompraService : ICarritoCompraService
{
    private readonly ICarritoCompraRepositorio _CarritoCompraRepo;
    private readonly IMapper _mapper;

    private readonly ILogger<CarritoCompraService> _logger;
    private readonly IProductoTallaRepositorio _productoTallaRepo;
    private readonly CarritoServicioDominio _carritoServicioDominio;
    private readonly IValidator<CarritoCompraCreateDto> _createValidator;
    private readonly IValidator<CarritoCompraUpdateDto> _updateValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator; 

    public CarritoCompraService(ICarritoCompraRepositorio CarritoCompraRepo, IMapper mapper, ILogger<CarritoCompraService> logger , CarritoServicioDominio carritoServicioDominio, IProductoTallaRepositorio productoTallaRepo , IValidator<CarritoCompraCreateDto> createValidator, IValidator<CarritoCompraUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator )
    {
        _CarritoCompraRepo = CarritoCompraRepo;
        _mapper = mapper;
        _logger = logger;
        _carritoServicioDominio = carritoServicioDominio;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _productoTallaRepo = productoTallaRepo;

    }


    public async Task<ApiResponse<List<CarritoCompraDto>>> ObtenerTodosLosCarritoCompraAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los CarritoCompras...");

            var carritoCompras = await _CarritoCompraRepo.ObtenerCarritoCompraConDetalles();

            if (carritoCompras == null || !carritoCompras.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron CarritoCompras registrados.");
                return ResponseHelper.Fail<List<CarritoCompraDto>>(
                    new List<ErrorDetail>
                    {
                    new() { Campo = "CarritoCompras", Mensaje = "No se encontraron CarritoCompras registrados." }
                    },
                    HttpStatusCode.NotFound
                );
            }

            // Se calculan subtotales y total general del carrito.
            _carritoServicioDominio.CalcularSubtotales(carritoCompras);
            decimal totalGeneral = _carritoServicioDominio.CalcularTotal(carritoCompras);

            foreach (var item in carritoCompras)
                item.TotalCarrito = totalGeneral;

            var carritoComprasDto = _mapper.Map<List<CarritoCompraDto>>(carritoCompras);

            _logger.LogInformation("✅ CarritoCompras obtenidos exitosamente.");

            return ResponseHelper.Success(carritoComprasDto, "CarritoCompras obtenidos correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener CarritoCompras.");
            return ResponseHelper.FailException<List<CarritoCompraDto>>(ex);
        }
    }


    public async Task<ApiResponse<object>> ObtenerTodosLosCarritoCompraUsuarioAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("🛒 Obteniendo CarritoCompras del usuario ID {UsuarioId}...", usuarioId);

            var carrito = await _CarritoCompraRepo.ObtenerCarritoPorUsuarioConDetalles(usuarioId);

            if (carrito == null || !carrito.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron CarritoCompras para el usuario ID {UsuarioId}.", usuarioId);
                return ResponseHelper.Success<object>(
                    new
                    {
                        Items = new List<CarritoCompraDto>(),
                        TotalCarrito = 0m
                    },
                    "El usuario no tiene CarritoCompras registrados.",
                    HttpStatusCode.OK
                );
            }

            // Cálculo de totales del carrito del usuario.
            _carritoServicioDominio.CalcularSubtotales(carrito);
            decimal totalCarrito = _carritoServicioDominio.CalcularTotal(carrito);

            foreach (var item in carrito)
                item.TotalCarrito = totalCarrito;

            var carritoDto = _mapper.Map<List<CarritoCompraDto>>(carrito);

            _logger.LogInformation("✅ CarritoCompras del usuario ID {UsuarioId} obtenidos correctamente.", usuarioId);

            return ResponseHelper.Success<object>(
                new { Items = carritoDto, TotalCarrito = totalCarrito },
                "CarritoCompras obtenidos correctamente.",
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener CarritoCompras del usuario ID {UsuarioId}.", usuarioId);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<CarritoCompraDto>> ObtenerCarritoCompraPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CarritoCompraDto>(validation.Errors);

            var CarritoCompra = await _CarritoCompraRepo.Obtener(a => a.CarritoId == id);
            if (CarritoCompra == null)
            {
                _logger.LogWarning("⚠️ No se encontró el CarritoCompra con ID {Id}.", id);
                return ResponseHelper.Fail<CarritoCompraDto>(
                    new List<ErrorDetail>
                    {
                    new() { Campo = "Id", Mensaje = $"No se encontró el CarritoCompra con ID {id}." }
                    },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<CarritoCompraDto>(CarritoCompra);
            _logger.LogInformation("✅ CarritoCompra con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "CarritoCompra encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener CarritoCompra por ID {Id}", id);
            return ResponseHelper.FailException<CarritoCompraDto>(ex);
        }
    }

    public async Task<ApiResponse<CarritoCompraDto>> CrearCarritoCompraAsync(CarritoCompraCreateDto createDto)
    {
        try
        {
            _logger.LogInformation("🛍️ Creando carrito para UsuarioId: {UsuarioId}", createDto.UsuarioId);

            // Validación del DTO
            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CarritoCompraDto>(validation.Errors);

            // Validar existencia de producto
            var productoTalla = await _productoTallaRepo.ObtenerProductoTallaConDetallesPorId(createDto.ProductoTallaId);
            if (productoTalla == null)
                return ResponseHelper.Fail<CarritoCompraDto>("Producto Talla no existe.", "ProductoTallaId");

            // Validar stock
            int stockDisponible = productoTalla.Stock - productoTalla.StockReservado;
            if (createDto.Cantidad > stockDisponible)
                return ResponseHelper.Fail<CarritoCompraDto>("No hay stock suficiente.", "Cantidad");

            // Reservar stock (solo si NO es invitado)
            bool stockReservado = await CarritoHelper.ReservarStockSiNoEsInvitadoAsync(_productoTallaRepo, createDto.ProductoTallaId, createDto.Cantidad, createDto.UsuarioId, _logger);
            if (!stockReservado)
                return ResponseHelper.Fail<CarritoCompraDto>("No se pudo reservar stock.", "Cantidad");

            // Verificar si ya existe el ítem en el carrito
            var existente = await _CarritoCompraRepo.Obtener(x => x.UsuarioId == createDto.UsuarioId && x.ProductoTallaId == createDto.ProductoTallaId);
            if (existente != null)
            {
                existente.Cantidad += createDto.Cantidad;
                existente.SubTotal = existente.Cantidad * existente.PrecioUnitario;
                await _CarritoCompraRepo.ActualizarCarritoCompra(existente);
                return ResponseHelper.Success(_mapper.Map<CarritoCompraDto>(existente), "Carrito actualizado correctamente.");
            }

            // Crear nuevo item
            var nuevoCarrito = new CarritoCompra
            {
                UsuarioId = createDto.UsuarioId,
                ProductoTallaId = createDto.ProductoTallaId,
                Cantidad = createDto.Cantidad,
                PrecioUnitario = createDto.PrecioUnitario,
                SubTotal = createDto.Cantidad * createDto.PrecioUnitario,
                Estado = createDto.Estado
            };

            await _CarritoCompraRepo.Crear(nuevoCarrito);
            return ResponseHelper.Success(_mapper.Map<CarritoCompraDto>(nuevoCarrito), "Carrito creado correctamente.", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creando carrito para UsuarioId: {UsuarioId}", createDto?.UsuarioId);
            return ResponseHelper.FailException<CarritoCompraDto>(ex);
        }
    }


    public async Task<ApiResponse<object>> EliminarCarritoCompraAsync(int id)
    {
        try
        {
            _logger.LogInformation("🗑️ Iniciando eliminación del carrito con ID {Id}", id);

            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new ErrorDetail { Campo = e.PropertyName, Mensaje = e.ErrorMessage })
                    .ToList();

                _logger.LogWarning("❌ Validación fallida al eliminar carrito {Id}: {Errores}", id, string.Join(", ", errores.Select(e => e.Mensaje)));
                return ResponseHelper.Fail<object>(errores, HttpStatusCode.BadRequest);
            }

            var carritoExistente = await _CarritoCompraRepo.Obtener(c => c.CarritoId == id);
            if (carritoExistente == null)
            {
                _logger.LogWarning("⚠️ CarritoCompra con ID {Id} no encontrado.", id);
                return ResponseHelper.Fail<object>("El item no existe en el carrito.", "CarritoId", HttpStatusCode.NotFound);
            }

            // Liberar stock
            await _productoTallaRepo.LiberarStockAsync(carritoExistente.ProductoTallaId, carritoExistente.Cantidad);

            // Eliminar item
            await _CarritoCompraRepo.Remover(carritoExistente);

            // Recalcular total
            decimal totalCarrito = await _CarritoCompraRepo.CalcularTotalAsync(carritoExistente.UsuarioId);
            var resultado = new { Eliminado = carritoExistente, TotalCarrito = totalCarrito };

            _logger.LogInformation("🗑️ CarritoCompra con ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(resultado, "Item del carrito eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar CarritoCompra con ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<object>> ActualizarCarritoCompraAsync(int id, CarritoCompraUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<object>("Datos inválidos para actualizar el carrito.", "CarritoCompra");

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var carritoExistente = await _CarritoCompraRepo.Obtener(c => c.CarritoId == id, tracked: true);
            if (carritoExistente == null)
                return ResponseHelper.Fail<object>("Carrito no encontrado.", "CarritoId", HttpStatusCode.NotFound);

            int diferencia = updateDto.Cantidad - carritoExistente.Cantidad;

            if (diferencia > 0)
            {
                bool stockDisponible = await CarritoHelper.ReservarStockSiNoEsInvitadoAsync(
                    _productoTallaRepo, carritoExistente.ProductoTallaId, diferencia, carritoExistente.UsuarioId, _logger);

                if (!stockDisponible)
                    return ResponseHelper.Fail<object>("No hay suficiente stock para aumentar la cantidad.", "Stock");
            }
            else if (diferencia < 0)
            {
                await CarritoHelper.LiberarStockSiNoEsInvitadoAsync(
                    _productoTallaRepo, carritoExistente.ProductoTallaId, -diferencia, carritoExistente.UsuarioId, _logger);
            }

            carritoExistente.Cantidad = updateDto.Cantidad > 0 ? updateDto.Cantidad : carritoExistente.Cantidad;
            carritoExistente.PrecioUnitario = updateDto.PrecioUnitario > 0 ? updateDto.PrecioUnitario : carritoExistente.PrecioUnitario;
            carritoExistente.SubTotal = carritoExistente.Cantidad * carritoExistente.PrecioUnitario;
            carritoExistente.Estado = updateDto.Estado ?? carritoExistente.Estado;

            await _CarritoCompraRepo.ActualizarCarritoCompra(carritoExistente);
            decimal totalCarrito = await _CarritoCompraRepo.CalcularTotalAsync(carritoExistente.UsuarioId);

            // 🔑 Aquí convertimos explícitamente a object
            object resultado = new { Item = carritoExistente, TotalCarrito = totalCarrito };

            return ResponseHelper.Success(resultado, "Carrito actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error actualizando carrito ID: {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }




    public async Task<ApiResponse<CarritoCompraDto>> ActualizarParcialCarritoCompraAsync(int id, JsonPatchDocument<CarritoCompraUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<CarritoCompraDto>("Datos inválidos para actualización parcial.", "Patch");

            var carritoExistente = await _CarritoCompraRepo.Obtener(c => c.CarritoId == id, tracked: true);
            if (carritoExistente == null)
                return ResponseHelper.Fail<CarritoCompraDto>("Carrito no encontrado.", "Id", HttpStatusCode.NotFound);

            var carritoDto = _mapper.Map<CarritoCompraUpdateDto>(carritoExistente);
            patchDto.ApplyTo(carritoDto);

            var validation = await _updateValidator.ValidateAsync(carritoDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<CarritoCompraDto>(validation.Errors);

            int diferencia = carritoDto.Cantidad - carritoExistente.Cantidad;

            if (diferencia > 0)
            {
                bool stockDisponible = await CarritoHelper.ReservarStockSiNoEsInvitadoAsync(_productoTallaRepo, carritoExistente.ProductoTallaId, diferencia, carritoExistente.UsuarioId, _logger);
                if (!stockDisponible)
                    return ResponseHelper.Fail<CarritoCompraDto>("No hay suficiente stock para aumentar la cantidad.", "Stock");
            }
            else if (diferencia < 0)
            {
                await CarritoHelper.LiberarStockSiNoEsInvitadoAsync(_productoTallaRepo, carritoExistente.ProductoTallaId, -diferencia, carritoExistente.UsuarioId, _logger);
            }

            carritoExistente.Cantidad = carritoDto.Cantidad;
            carritoExistente.PrecioUnitario = carritoDto.PrecioUnitario;
            carritoExistente.SubTotal = carritoExistente.Cantidad * carritoExistente.PrecioUnitario;
            carritoExistente.Estado = carritoDto.Estado ?? carritoExistente.Estado;

            await _CarritoCompraRepo.ActualizarCarritoCompra(carritoExistente);

            return ResponseHelper.Success(_mapper.Map<CarritoCompraDto>(carritoExistente), "Carrito actualizado parcialmente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error aplicando PATCH carrito ID: {Id}", id);
            return ResponseHelper.FailException<CarritoCompraDto>(ex);
        }
    }


    public async Task<ApiResponse<CarritoCompraDto>> ObtenerSubtotalAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("🧮 Calculando subtotal para el usuario {UsuarioId}", usuarioId);

            var carrito = await _CarritoCompraRepo.ObtenerCarritoPorUsuarioConDetalles(usuarioId);

            //  Si el carrito está vacío, devolvemos subtotal = 0
            if (carrito == null || !carrito.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron items en el carrito del usuario {UsuarioId}.", usuarioId);
                return ResponseHelper.Success<CarritoCompraDto>(
                    null,
                    "El carrito está vacío. Subtotal = 0.",
                    HttpStatusCode.OK
                );
            }

            // Reutilizamos la lógica del dominio
            _carritoServicioDominio.CalcularSubtotales(carrito);
            decimal subtotal = _carritoServicioDominio.CalcularTotal(carrito);

            _logger.LogInformation("✅ Subtotal calculado correctamente: {Subtotal}", subtotal);

            return ResponseHelper.Success<CarritoCompraDto>(
                new CarritoCompraDto { TotalCarrito = subtotal },
                "Subtotal calculado correctamente",
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al calcular subtotal del carrito para el usuario {UsuarioId}", usuarioId);
            return ResponseHelper.FailException<CarritoCompraDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> VaciarCarritoAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("🛒 Vaciando carrito para el usuario {UsuarioId}", usuarioId);

            if (usuarioId <= 0)
                return ResponseHelper.Fail<object>(
                    new List<ErrorDetail> { new() { Campo = "UsuarioId", Mensaje = "El identificador del usuario no es válido." } },
                    HttpStatusCode.BadRequest
                );

            var carrito = await _CarritoCompraRepo.ObtenerCarritoPorUsuarioConDetalles(usuarioId);
            if (carrito == null || !carrito.Any())
                return ResponseHelper.Success<object>(null, "El carrito ya está vacío.", HttpStatusCode.OK);

            // Se libera el stock de todos los ítems antes de eliminarlos.
            foreach (var item in carrito)
            {
                await _productoTallaRepo.LiberarStockAsync(item.ProductoTallaId, item.Cantidad);
            }

            // Vaciar carrito
            await _CarritoCompraRepo.VaciarCarritoPorUsuarioAsync(usuarioId);

            _logger.LogInformation("✅ Carrito del usuario {UsuarioId} vaciado y stock liberado correctamente", usuarioId);
            return ResponseHelper.Success<object>(null, $"Carrito del usuario {usuarioId} vaciado correctamente.", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al vaciar el carrito del usuario {UsuarioId}", usuarioId);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<decimal>> ObtenerTotalCarritoAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("🧮 Calculando total del carrito para el usuario {UsuarioId}", usuarioId);

            if (usuarioId <= 0)
            {
                return ResponseHelper.Fail<decimal>(
                    new List<ErrorDetail> { new() { Campo = "UsuarioId", Mensaje = "El identificador del usuario no es válido." } },
                    HttpStatusCode.BadRequest
                );
            }

            var carrito = await _CarritoCompraRepo.ObtenerCarritoPorUsuarioConDetalles(usuarioId);
            if (carrito == null || !carrito.Any())
            {
                _logger.LogWarning("⚠️ No se encontró carrito para el usuario {UsuarioId}", usuarioId);
                return ResponseHelper.Success(0m, "El carrito está vacío.", HttpStatusCode.OK);
            }

            // Reutilizamos lógica de dominio
            _carritoServicioDominio.CalcularSubtotales(carrito);
            decimal totalCarrito = _carritoServicioDominio.CalcularTotal(carrito);

            // (Opcional) actualizar el total en cada item
            foreach (var item in carrito)
                item.TotalCarrito = totalCarrito;

            await _CarritoCompraRepo.ActualizarVariosAsync(carrito);

            _logger.LogInformation("✅ Total del carrito calculado correctamente: {Total}", totalCarrito);
            return ResponseHelper.Success(totalCarrito, "Total del carrito calculado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al calcular el total del carrito para el usuario {UsuarioId}", usuarioId);
            return ResponseHelper.FailException<decimal>(ex);
        }
    }

    public async Task<ApiResponse<object>> ConfirmarCompraCarritoAsync(int usuarioId)
    {
        try
        {
            _logger.LogInformation("💳 Confirmando compra para el usuario {UsuarioId}", usuarioId);

            if (usuarioId <= 0)
            {
                return ResponseHelper.Fail<object>(
                    new List<ErrorDetail>
                    {
                    new() { Campo = "UsuarioId", Mensaje = "El identificador del usuario no es válido." }
                    },
                    HttpStatusCode.BadRequest
                );
            }

            // 1️⃣ Obtener carrito
            var carrito = await _CarritoCompraRepo.ObtenerCarritoPorUsuarioConDetalles(usuarioId);

            if (carrito == null || !carrito.Any())
            {
                return ResponseHelper.Fail<object>(
                    new List<ErrorDetail>
                    {
                    new() { Campo = "Carrito", Mensaje = "No hay items en el carrito para procesar la compra." }
                    },
                    HttpStatusCode.BadRequest
                );
            }

            // 2️⃣ LIMPIAR PRODUCTOS FANTASMA / SIN STOCK
            foreach (var item in carrito.ToList()) // 🔥 ToList para evitar errores
            {
                if (item.ProductoTalla == null)
                {
                    _logger.LogWarning(
                        "⚠️ Producto inválido eliminado del carrito. ProductoTallaId: {Id}",
                        item.ProductoTallaId
                    );

                    await _CarritoCompraRepo.EliminarItemAsync(item.CarritoId);
                    carrito.Remove(item);
                    continue;
                }

                if (item.ProductoTalla.Stock <= 0)
                {
                    _logger.LogWarning(
                        "⚠️ Producto sin stock eliminado del carrito. ProductoTallaId: {Id}",
                        item.ProductoTallaId
                    );

                    await _CarritoCompraRepo.EliminarItemAsync(item.CarritoId);
                    carrito.Remove(item);
                }
            }

            // 3️⃣ Revalidar carrito después de limpieza
            if (!carrito.Any())
            {
                return ResponseHelper.Fail<object>(
                    new List<ErrorDetail>
                    {
                    new()
                    {
                        Campo = "Carrito",
                        Mensaje = "El carrito quedó vacío porque los productos no eran válidos."
                    }
                    },
                    HttpStatusCode.BadRequest
                );
            }

            // 4️⃣ Calcular totales (NO SE TOCA TU LÓGICA)
            _carritoServicioDominio.CalcularSubtotales(carrito);
            decimal totalCompra = _carritoServicioDominio.CalcularTotal(carrito);

            // 5️⃣ Vaciar carrito (flujo correcto)
            await _CarritoCompraRepo.VaciarCarritoPorUsuarioAsync(usuarioId);

            _logger.LogInformation(
                "🧹 Carrito limpiado correctamente para el usuario {UsuarioId}",
                usuarioId
            );

            // 6️⃣ Respuesta final
            return ResponseHelper.Success<object>(
                new
                {
                    UsuarioId = usuarioId,
                    TotalCompra = totalCompra
                },
                "Compra confirmada y carrito limpiado correctamente.",
                HttpStatusCode.OK
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al confirmar compra para el usuario {UsuarioId}", usuarioId);
            return ResponseHelper.FailException<object>(ex);
        }
    }


   




}

