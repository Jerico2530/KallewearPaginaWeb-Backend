using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using System.Linq.Expressions;
/*
    * ICarritoCompraRepositorio
    *
    * Interfaz de repositorio especializada en la gestión de CarritoCompra.
    * Funcionalidades clave:
    * - CRUD básico y operaciones avanzadas sobre el carrito de compras.
    * - Obtener carritos con detalles completos por usuario o ID.
    * - Vaciar carritos y calcular totales.
    * - Soporta operaciones genéricas y migración de carritos.
    *
    * Propósito del componente:
    * Centralizar todas las operaciones de acceso a datos relacionadas con los carritos de compra,
    * asegurando consistencia, eficiencia y encapsulación de la lógica de persistencia.
    * Actúa como capa de abstracción entre la base de datos y los servicios de negocio,
    * manteniendo el código desacoplado, limpio y fácil de mantener.
    */
namespace ApiRopa;

public interface ICarritoCompraRepositorio : IRepositorio<CarritoCompra>
{
    //Actualiza un carrito de compra existente y devuelve la entidad actualizada.
    Task<CarritoCompra> ActualizarCarritoCompra(CarritoCompra entidad);
    //Obtiene todos los carritos con sus detalles completos.
    Task<List<CarritoCompra>> ObtenerCarritoCompraConDetalles();
    // Obtiene un carrito con detalles por su ID.
    Task<CarritoCompra> ObtenerCarritoCompraConDetallesPorId(int id);
    // Obtiene los carritos de un usuario específico con detalles.
    Task<List<CarritoCompra>> ObtenerCarritoPorUsuarioConDetalles(int usuarioId);
    //Vacía el carrito de un usuario.
    Task VaciarCarritoPorUsuarioAsync(int usuarioId);
    // Calcula el total del carrito de un usuario.
    Task<decimal> CalcularTotalAsync(int usuarioId);

    //Obtiene una lista genérica filtrada de carritos (para migraciones o usos específicos).
    Task<IEnumerable<CarritoCompra>> ObtenerTodos(
        Expression<Func<CarritoCompra, bool>> filtro);

    //Actualiza un carrito genéricamente (para migraciones u operaciones específicas).
    Task Actualizar(CarritoCompra entidad);

    //Obtiene el ID del usuario invitado actual (depende de la implementación de invitado).
    Task<int> ObtenerIdInvitadoActualAsync();

}

