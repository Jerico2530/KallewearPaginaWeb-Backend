using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;

public interface IOrdenCuponRepositorio : IRepositorio<OrdenCupon>
{
    Task<OrdenCupon> ActualizarOrdenCupon(OrdenCupon entidad);
}
