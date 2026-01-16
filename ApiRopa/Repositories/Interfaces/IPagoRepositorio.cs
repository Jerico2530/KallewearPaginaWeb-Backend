using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
/*
     * IPagoRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de pagos dentro del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD sobre la entidad Pago.
     * - Recuperación de pagos junto con sus detalles relacionados.
     *
     * Propósito del componente:
     * Proporcionar un acceso centralizado y consistente a los datos de pagos, asegurando
     * integridad en las operaciones y facilitando la interacción de la capa de servicios
     * con la base de datos. Permite mantener el código limpio, desacoplado y fácilmente
     * mantenible.
     */
namespace ApiRopa;

public interface IPagoRepositorio : IRepositorio<Pago>
{
    /// Actualiza un pago existente y devuelve la entidad actualizada.
    Task<Pago> ActualizarPago(Pago entidad);
    /// Obtiene la lista de pagos incluyendo los detalles asociados.
    Task<List<Pago>> ObtenerPagosConDetalles();
    /// Obtiene la lista de pagos incluyendo los detalles asociados por usuarios.
    Task<List<Pago>> ObtenerPagosPorUsuario(int usuarioId);
}
