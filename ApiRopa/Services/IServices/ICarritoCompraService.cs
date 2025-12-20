using ApiRopa.Models.Responses;
using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using BiblotecaWeb.Model.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
/*
 * Servicio de gestión del carrito de compras dentro de la tienda de ropa.
 *
 * Esta interfaz define los contratos esenciales para la lógica de negocio asociada
 * al flujo de compra del usuario: administración de ítems del carrito, cálculo de importes
 * y confirmación de compra.
 *
 * Funcionalidades clave:
 * - CRUD completo del carrito de compras.
 * - Obtención del subtotal y total del carrito por usuario.
 * - Vaciar carrito y confirmar compra.
 * - Aplicar actualizaciones parciales mediante JsonPatch.
 *
 * Actúa como capa de abstracción entre controladores y la capa de persistencia,
 * asegurando consistencia en los datos y respuestas estandarizadas con ApiResponse.
 */
namespace ApiRopa;

public interface ICarritoCompraService
{
    // Obtiene todos los carritos registrados (uso administrativo o monitoreo)
    Task<ApiResponse<List<CarritoCompraDto>>> ObtenerTodosLosCarritoCompraAsync();
    // Obtiene el carrito completo de un usuario específico
    Task<ApiResponse<object>> ObtenerTodosLosCarritoCompraUsuarioAsync(int usuarioId);
    // Busca un carrito según su identificador principal
    Task<ApiResponse<CarritoCompraDto>> ObtenerCarritoCompraPorIdAsync(int id);
    // Registra un nuevo carrito asociado a un usuario
    Task<ApiResponse<CarritoCompraDto>> CrearCarritoCompraAsync(CarritoCompraCreateDto dto);
    // Actualiza un carrito existente reemplazando su contenido
    Task<ApiResponse<object>> ActualizarCarritoCompraAsync(int id, CarritoCompraUpdateDto updateDto);
    // Realiza actualizaciones parciales sobre un carrito usando JsonPatch
    Task<ApiResponse<CarritoCompraDto>> ActualizarParcialCarritoCompraAsync(int id, JsonPatchDocument<CarritoCompraUpdateDto> patchDto);
    // Elimina un carrito de la base de datos
    Task<ApiResponse<object>> EliminarCarritoCompraAsync(int id);
    // Calcula el subtotal acumulado de todos los ítems del usuario
    Task<ApiResponse<CarritoCompraDto>> ObtenerSubtotalAsync(int usuarioId);
    // Limpia todos los ítems del carrito del usuario
    Task<ApiResponse<object>> VaciarCarritoAsync(int usuarioId);
    // Obtiene el total final del carrito del usuario (descuentos e impuestos gestionados por la capa de negocio)
    Task<ApiResponse<decimal>> ObtenerTotalCarritoAsync(int usuarioId);
    // Confirma el proceso de compra del carrito del usuario
    Task<ApiResponse<object>> ConfirmarCompraCarritoAsync(int usuarioId);





}

