using BiblotecaWeb;

namespace ApiRopa.Repositorio.IRepositorio;
/*
     * IPermisoRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de permisos del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD sobre la entidad Permiso.
     * - Actualización de permisos existentes en la base de datos.
     *
     * Propósito del componente:
     * Centralizar el acceso y la gestión de permisos, asegurando integridad y consistencia
     * de los datos, facilitando que la capa de servicios interactúe de forma confiable
     * con la base de datos. Mantiene el código limpio, desacoplado y fácilmente mantenible.
     */
public interface IPermisoRepositorio : IRepositorio<Permiso>
{
    /// Actualiza un permiso existente y devuelve la entidad actualizada.
    Task<Permiso> ActualizarPermiso(Permiso entidad);
}
