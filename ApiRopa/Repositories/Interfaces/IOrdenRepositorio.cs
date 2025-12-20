using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
     * IOrdenRepositorio
     *
     * Interfaz de repositorio dedicada a la gestión de órdenes dentro del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD sobre la entidad Orden.
     * - Recuperar órdenes con sus detalles completos.
     * - Manejo de carritos de compra asociados a usuarios y órdenes.
     * - Actualización masiva de carritos vinculados a órdenes.
     *
     * Propósito del componente:
     * Centralizar el acceso y la manipulación de datos de órdenes, proporcionando métodos
     * especializados que aseguran integridad y consistencia entre órdenes y carritos de compra.
     * Facilita la interacción de la capa de servicios con la base de datos manteniendo el código
     * limpio, desacoplado y reutilizable.
     */
namespace ApiRopa;

public interface IOrdenRepositorio :IRepositorio<Orden>
{
    /// Actualiza una orden existente y devuelve la entidad actualizada.
    Task<Orden> ActualizarOrden(Orden entidad);
    /// Obtiene la lista de carritos de compra asociados a órdenes.
    Task<List<Orden>> ObtenerCarritoCompraConDetalles();
    /// Obtiene una orden específica junto con todos sus detalles.
    Task<Orden> ObtenerOrdenConDetallesPorIdAsync(int ordenId);
    /// Obtiene los carritos de un usuario que aún no han sido asociados a ninguna orden.
    Task<List<CarritoCompra>> ObtenerCarritoSinOrden(int usuarioId);
    /// Actualiza múltiples carritos de manera masiva.
    Task ActualizarCarritos(List<CarritoCompra> carritos);

}
