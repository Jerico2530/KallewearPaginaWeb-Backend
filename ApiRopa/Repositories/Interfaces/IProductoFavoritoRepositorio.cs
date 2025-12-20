using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;

public interface IProductoFavoritoRepositorio : IRepositorio<ProductoFavorito>
{
    Task<ProductoFavorito> ActualizarProductoFavorito(ProductoFavorito entidad);
    Task<List<ProductoFavorito>> ObtenerProductoFavoritoConDetalles();
}

