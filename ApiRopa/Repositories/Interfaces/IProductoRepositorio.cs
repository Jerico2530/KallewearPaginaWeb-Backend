using BiblotecaWeb.Domain.Entities;
/*
    * IProductoRepositorio
    *
    * Interfaz de repositorio especializada en la gestión de la entidad Producto.
    * Funcionalidades clave:
    * - Operaciones CRUD sobre Productos.
    * - Obtención de listas de Productos con sus detalles completos.
    * - Recuperación de un Producto específico junto con todas sus relaciones.
    *
    * Propósito del componente:
    * Centralizar y abstraer el acceso a los datos de Producto, asegurando
    * consistencia y mantenibilidad. Actúa como capa intermedia entre los servicios
    * de negocio y la base de datos, permitiendo operaciones claras, desacopladas
    * y fáciles de testear.
    */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {
        /// Actualiza un Producto existente y devuelve la entidad actualizada.
        Task<Producto> ActualizarProducto(Producto entidad);
        /// Obtiene todos los Productos incluyendo sus relaciones y detalles completos.
        Task<List<Producto>> ObtenerProductosConDetalles();
        /// Obtiene un Producto específico con todos sus detalles por su ID.
        Task<Producto> ObtenerProductoConDetallesPorId(int id);
    }
}
