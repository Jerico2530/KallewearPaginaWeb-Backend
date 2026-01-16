using ApiRopa.Repositorio.IRepositorio;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa.Repositories.Interfaces
{
    public interface IInfoTarjetaRepositorio : IRepositorio<InfoTarjetas>
    {
        Task<InfoTarjetas> ActualizarInfoTarjeta(InfoTarjetas entidad);
        Task<List<InfoTarjetas>> ObtenerInfoTarjetasConDetalles();
        Task<List<InfoTarjetas>> ObtenerInfoTarjetasPorUsuarioAsync(int usuarioId);
    }
}
