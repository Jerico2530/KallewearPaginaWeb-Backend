using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa;

public interface IMedioPagoRepositorio : IRepositorio<MedioPago>
{
    Task<MedioPago> ActualizarMedioPago(MedioPago entidad);
    Task<List<MedioPago>> ObtenerMedioPagosConDetalles();
}
