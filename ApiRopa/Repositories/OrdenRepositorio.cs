using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;
using System;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * OrdenRepositorio
     *
     * Repositorio especializado para la gestión de órdenes dentro del sistema.
     *
     * Funcionalidades clave:
     * - Administración completa de las órdenes realizadas por los usuarios.
     * - Acceso a información relacional: usuario, sucursal, dirección y productos del carrito.
     * - Optimización de consultas con Entity Framework Core mediante carga explícita de datos relacionados.
     *
     * Propósito del componente:
     * Garantizar la obtención, actualización y manipulación eficiente de las órdenes y su información asociada,
     * manteniendo consistencia en el flujo de compra.
     *
     * Descripción del código:
     * - Se heredan operaciones CRUD del repositorio genérico.
     * - Se implementan métodos personalizados para obtener órdenes con detalles
     *   y gestionar carritos de compra vinculados o no vinculados a una orden.
     */
public class OrdenRepositorio : Repositorio<Orden>, IOrdenRepositorio
{
    private readonly AppDbContext _db; // Contexto de base de datos para la persistencia

    public OrdenRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una orden existente en la base de datos.
    public async Task<Orden> ActualizarOrden(Orden entidad)
    {
        _db.Ordenes.Update(entidad);         // Marca la entidad como modificada
        await _db.SaveChangesAsync();      // Persiste los cambios
        return entidad;                    // Devuelve la orden actualizada
    }
    /// Obtiene todas las órdenes con datos relacionados del usuario y su carrito,
    /// optimizando la consulta sin rastreo.
    public async Task<List<Orden>> ObtenerCarritoCompraConDetalles()
    {
        return await _db.Ordenes
            .AsNoTracking()                  // Mejora el rendimiento al no rastrear entidades
                .Include(o => o.Usuario)         // Incluye información del usuario
                .Include(o => o.Sucursal)        // Incluye datos de la sucursal
                .Include(o => o.Direccion)       // Incluye datos de la dirección de entrega
            .ToListAsync();
    }

    /// Obtiene una orden específica con todos sus detalles relacionados.
    /// Incluye navegación a productos y tallas dentro del carrito de compras.
    public async Task<Orden> ObtenerOrdenConDetallesPorIdAsync(int ordenId)
    {
        return await _db.Ordenes
            .AsNoTracking()
            .Include(o => o.Usuario)
            .Include(o => o.Sucursal)
            .Include(o => o.Direccion)
            .Include(o => o.CarritoCompras)       
                .ThenInclude(c => c.ProductoTalla)
                .ThenInclude(p => p.Producto)
            .Include(o => o.CarritoCompras)
                .ThenInclude(c => c.ProductoTalla)
                .ThenInclude(p => p.Talla)     // Incluir información de la talla
            .FirstOrDefaultAsync(o => o.OrdenId == ordenId);
    }

    /// Obtiene los productos del carrito que aún no están vinculados a una orden.
    /// Útil durante el proceso de compra previo a la confirmación.
    public async Task<List<CarritoCompra>> ObtenerCarritoSinOrden(int usuarioId)
    {
        return await _db.CarritoCompras
                .Include(c => c.ProductoTalla)
                .ThenInclude(p => p.Producto) // Para poder usar c.Producto.Precio
            .Where(c => c.UsuarioId == usuarioId && c.OrdenId == null)
            .ToListAsync();
    }

    /// Actualiza múltiples carritos de compra simultáneamente.
    /// Implementado para optimizar operaciones por lote.
    public async Task ActualizarCarritos(List<CarritoCompra> carritos)
    {
        _db.CarritoCompras.UpdateRange(carritos); // Actualización masiva
        await _db.SaveChangesAsync();            // Guarda los cambios en la base de datos
    }


}
