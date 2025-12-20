using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
     * IOrdenDetalleRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de los detalles de órdenes dentro del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre la entidad OrdenDetalle.
     * - Actualización de registros existentes con control de cambios.
     *
     * Propósito del componente:
     * Abstraer el acceso a datos de los detalles de las órdenes, facilitando la interacción
     * de la capa de servicios con la base de datos y garantizando consistencia y reutilización
     * de la lógica de acceso a datos.
     */
namespace ApiRopa;

public interface IOrdenDetalleRepositorio : IRepositorio<OrdenDetalle>
{
    /// Actualiza un detalle de orden existente y devuelve la entidad actualizada.
    Task<OrdenDetalle> ActualizarOrdenDetalle(OrdenDetalle entidad);
}
