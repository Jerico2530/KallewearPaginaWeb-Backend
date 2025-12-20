using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
/*
     * ProductoRepositorio
     *
     * Repositorio especializado para la administración de productos dentro del catálogo del sistema.
     *
     * Funcionalidades clave:
     * - Actualización de datos de productos.
     * - Obtención de productos con información complementaria asociada.
     *
     * Propósito del componente:
     * Centralizar y encapsular las operaciones de persistencia sobre productos,
     * asegurando un acceso eficiente a la información del catálogo y favoreciendo la escalabilidad.
     *
     * Descripción del código:
     * - Implementa métodos específicos para la gestión de productos.
     * - Emplea Entity Framework Core para la inclusión de datos relacionados mediante carga anticipada.
     */
namespace ApiRopa.Repositorio
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        private readonly AppDbContext _db; // Contexto de acceso a datos del sistema

        public ProductoRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un producto existente en la base de datos.
        public async Task<Producto> ActualizarProducto(Producto entidad)
        {
            _db.Productos.Update(entidad); // Marca la entidad como modificada para persistir cambios
            await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos
            return entidad;               // Devuelve la entidad actualizada
        }
        /// Obtiene todos los productos con los detalles de género y moneda asociados.
        public async Task<List<Producto>> ObtenerProductosConDetalles()
        {
            return await _db.Productos
                .Include(ur => ur.Genero)
                .Include(ur => ur.Moneda)
                .ToListAsync();
        }
        /// Obtiene un producto específico por Id con sus datos relacionados.
        public async Task<Producto> ObtenerProductoConDetallesPorId(int id)
        {
            return await _db.Productos
                .Include(p => p.Genero)
                .Include(p => p.Moneda)
                .FirstOrDefaultAsync(p => p.ProductoId == id);
        }
    }
}
