using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;
using BiblotecaWeb.Model;

namespace ApiRopa;

public interface ICuponRepositorio : IRepositorio<Cupon>
{
    Task<Cupon> ActualizarCupon(Cupon entidad);
}
