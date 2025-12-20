using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;

public class ProductoFavoritoRepositorio : Repositorio<ProductoFavorito>, IProductoFavoritoRepositorio
{
    private readonly AppDbContext _db;

    public ProductoFavoritoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    public async Task<ProductoFavorito> ActualizarProductoFavorito(ProductoFavorito entidad)
    {
        _db.ProductoFavoritos.Update(entidad);
        await _db.SaveChangesAsync();
        return entidad;
    }
    public async Task<List<ProductoFavorito>> ObtenerProductoFavoritoConDetalles()
    {
        return await _db.ProductoFavoritos
            .Include(ur => ur.Producto)
            .Include(ur => ur.Usuario)
            .ToListAsync();
    }
}

