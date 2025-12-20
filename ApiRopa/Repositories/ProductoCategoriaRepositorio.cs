using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * ProductoCategoriaRepositorio
     *
     * Repositorio especializado para la gestión de la relación entre Productos y Categorías.
     *
     * Funcionalidades clave:
     * - Actualización de relaciones Producto-Categoría.
     * - Obtención de asignaciones con detalles completos de las entidades asociadas.
     *
     * Propósito del componente:
     * Mantener de forma consistente y escalable la clasificación de productos dentro del catálogo,
     * permitiendo gestionar qué categorías pertenecen a cada producto.
     *
     * Descripción del código:
     * - Implementa operaciones específicas sobre la entidad intermedia ProductoCategoria.
     * - Utiliza Entity Framework Core para optimizar consultas mediante carga anticipada (Include).
     */
public class ProductoCategoriaRepositorio : Repositorio<ProductoCategoria>, IProductoCategoriaRepositorio
{
    private readonly AppDbContext _db; // Contexto de acceso a la base de datos

    public ProductoCategoriaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una relación Producto-Categoría existente.
    public async Task<ProductoCategoria> ActualizarProductoCategoria(ProductoCategoria entidad)
    {
        _db.ProductoCategorias.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync();         // Persiste los cambios en la base de datos
        return entidad;                       // Retorna la entidad actualizada
    }
    /// Obtiene todas las relaciones Producto-Categoría con los detalles completos.
    public async Task<List<ProductoCategoria>> ObtenerProductoCategoriaConDetalles()
    {
        return await _db.ProductoCategorias
            .AsNoTracking()
            .Include(ur => ur.Producto)
            .Include(ur => ur.Categoria)
            .ToListAsync();
    }
    /// Obtiene una relación específica Producto-Categoría por Id con sus datos relacionados.
    public async Task<ProductoCategoria?> ObtenerProductoCategoriaConDetallesPorId(int id)
    {
        return await _db.ProductoCategorias
            .Include(ur => ur.Producto)
            .Include(ur => ur.Categoria)
            .FirstOrDefaultAsync(ur => ur.ProductoCategoriaId ==id);
    }


}

