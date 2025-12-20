using ApiRopa.Repositorio;
using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiRopa;
/*
    * ProductoTallaRepositorio
    *
    * Repositorio especializado para la administración de stock por talla asociado a los productos.
    *
    * Funcionalidades clave:
    * - Actualización de información de stock.
    * - Gestión de reserva, liberación y confirmación de unidades en inventario.
    * - Obtención de datos ampliados con información del producto, talla, categoría y moneda.
    *
    * Propósito del componente:
    * Centralizar el control del inventario a nivel de talla,
    * garantizando disponibilidad real y evitando inconsistencias durante procesos de compra.
    *
    * Descripción del código:
    * - Implementa operaciones específicas de inventario basado en tallas.
    * - Utiliza Entity Framework Core con carga anticipada y seguimiento de entidades para manipulación transaccional.
    * - Incluye validaciones para mantener integridad en el stock reservado y el stock disponible.
    */
public class ProductoTallaRepositorio : Repositorio<ProductoTalla>, IProductoTallaRepositorio
{
    private readonly AppDbContext _db;

    public ProductoTallaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza la información de una entidad ProductoTalla existente.
    public async Task<ProductoTalla> ActualizarProductoTalla(ProductoTalla entidad)
    {
        _db.ProductoTallas.Update(entidad);  // Marca la entidad como modificada
        await _db.SaveChangesAsync();       // Persiste los cambios en la BD
        return entidad;                     // Retorna la entidad actualizada
    }
    /// Verifica si existe un registro ProductoTalla por identificador.
    public async Task<bool> ExistePorIdAsync(int productoTallaId)
    {
        return await _db.ProductoTallas.AnyAsync(p => p.ProductoTallaId == productoTallaId);
    }
    /// Obtiene todas las tallas de productos con todos sus detalles asociados.
    public async Task<List<ProductoTalla>> ObtenerProductoTallasConDetalles()
    {
        return await _db.ProductoTallas
            .Include(pt => pt.Producto.Moneda)
            .Include(pt => pt.Producto.Genero)
            .Include(pt => pt.Producto)
    .ThenInclude(p => p.ProductoCategorias)
        .ThenInclude(pc => pc.Categoria)

            .Include(pt => pt.Talla)
            .ToListAsync();
    }
    /// Obtiene un registro específico de ProductoTalla por Id con detalles relacionados.
    public async Task<ProductoTalla?> ObtenerProductoTallaConDetallesPorId(int id)
    {
        return await _db.ProductoTallas
            .Include(pt => pt.Producto.Moneda)
            .Include(pt => pt.Producto.Genero)
            .Include(pt => pt.Producto)
    .ThenInclude(p => p.ProductoCategorias)
        .ThenInclude(pc => pc.Categoria)

            .Include(pt => pt.Talla)
            .FirstOrDefaultAsync(pt => pt.ProductoTallaId == id);
    }
    /// Reserva stock disponible para un proceso de compra.
    public async Task<bool> ReservarStockAsync(int productoTallaId, int cantidad)
    {
        var productoTalla = await _db.ProductoTallas
            .FirstOrDefaultAsync(x => x.ProductoTallaId == productoTallaId); // TRACKED

        if (productoTalla == null)
            return false;

        // Calcula stock realmente disponible
        int disponible = productoTalla.Stock - productoTalla.StockReservado;

        if (disponible < cantidad)
            return false;// No se puede reservar más de lo disponible

        productoTalla.StockReservado += cantidad;// Incrementa la reserva del stock

        await _db.SaveChangesAsync();
        return true;
    }
    /// Libera unidades reservadas previamente cuando la compra no es realizada.
    public async Task<bool> LiberarStockAsync(int productoTallaId, int cantidad)
    {
        var productoTalla = await _db.ProductoTallas
            .FirstOrDefaultAsync(x => x.ProductoTallaId == productoTallaId);

        if (productoTalla == null)
            return false;

        productoTalla.StockReservado -= cantidad;
        // Asegura que nunca exista stock reservado negativo
        if (productoTalla.StockReservado < 0)
            productoTalla.StockReservado = 0;

        await _db.SaveChangesAsync();
        return true;
    }

    /// Confirma la compra y descuenta definitivamente el stock disponible.
    public async Task<bool> ConfirmarCompraAsync(int productoTallaId, int cantidad)
    {
        var productoTalla = await _db.ProductoTallas
            .FirstOrDefaultAsync(x => x.ProductoTallaId == productoTallaId);

        if (productoTalla == null)
            return false;

        if (productoTalla.Stock < cantidad)
            return false;// No permite descontar por debajo del stock real

        productoTalla.Stock -= cantidad;

        // Limpia el stock reservado para esa compra
        productoTalla.StockReservado -= cantidad;
        if (productoTalla.StockReservado < 0)
            productoTalla.StockReservado = 0;

        await _db.SaveChangesAsync();
        return true;
    }


}
