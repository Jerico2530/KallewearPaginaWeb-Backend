using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa;
/*
     * IProductoTallaRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de la entidad ProductoTalla.
     * Funcionalidades clave:
     * - Operaciones CRUD sobre ProductoTalla.
     * - Verificación de existencia de registros por ID.
     * - Gestión de stock: reservar, confirmar compra y liberar stock.
     * - Obtención de listas y detalles completos de ProductoTalla.
     *
     * Propósito del componente:
     * Proveer un acceso centralizado y consistente a los datos de tallas de productos,
     * asegurando que las operaciones de inventario y stock sean seguras y coherentes.
     * Esta interfaz actúa como capa de abstracción entre la lógica de negocio
     * y la base de datos, favoreciendo mantenibilidad, testabilidad y claridad en el código.
     */
public interface IProductoTallaRepositorio : IRepositorio<ProductoTalla>
{
    /// Actualiza un ProductoTalla existente y devuelve la entidad actualizada.
    Task<ProductoTalla> ActualizarProductoTalla(ProductoTalla entidad);
    /// Verifica si un ProductoTalla existe por su ID.
    Task<bool> ExistePorIdAsync(int productoTallaId);
    /// Obtiene todos los ProductoTallas incluyendo sus detalles completos.
    Task<List<ProductoTalla>> ObtenerProductoTallasConDetalles();
    /// Obtiene un ProductoTalla específico con detalles por su ID.
    Task<ProductoTalla?> ObtenerProductoTallaConDetallesPorId(int id);
    /// Reserva una cantidad específica de stock de un ProductoTalla.
    Task<bool> ReservarStockAsync(int productoTallaId, int cantidad);
    /// Confirma la compra de una cantidad de ProductoTalla y actualiza el stock.
    Task<bool> ConfirmarCompraAsync(int productoTallaId, int cantidad);
    /// Libera stock previamente reservado de un ProductoTalla.
    Task<bool> LiberarStockAsync(int productoTallaId, int cantidad);


}

