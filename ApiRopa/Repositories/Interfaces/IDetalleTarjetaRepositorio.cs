using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
   * IDetalleTarjetaRepositorio
   *
   * Interfaz de repositorio especializada en la gestión de detalles de tarjetas asociados a usuarios o transacciones.
   * Funcionalidades clave:
   * - Operaciones CRUD básicas sobre entidades de tipo DetalleTarjeta.
   * - Actualización de detalles de tarjetas existentes.
   * - Obtención de listas de detalles de tarjetas incluyendo información relacionada (con detalles completos).
   *
   * Propósito del componente:
   * Proporcionar un acceso a datos consistente y desacoplado para la entidad DetalleTarjeta,
   * facilitando la integración con la lógica de negocio y manteniendo la consistencia
   * y escalabilidad del sistema.
   */
namespace ApiRopa;

public interface IDetalleTarjetaRepositorio : IRepositorio<DetalleTarjeta>
{
    /// Actualiza un detalle de tarjeta existente y devuelve la entidad actualizada.
    Task<DetalleTarjeta> ActualizarDetalleTarjeta(DetalleTarjeta entidad);
    /// Obtiene la lista de detalles de tarjetas, incluyendo sus relaciones completas.
    Task<List<DetalleTarjeta>> ObtenerDetalleTarjetasConDetalles();
}
