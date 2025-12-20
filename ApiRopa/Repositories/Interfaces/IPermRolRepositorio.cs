using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa.Repositorio.IRepositorio;
/*
     * IPermRolRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de relaciones entre permisos y roles.
     * Funcionalidades clave:
     * - CRUD sobre la entidad PermRol.
     * - Obtener relaciones de permisos y roles con todos los detalles asociados.
     * - Consultar relaciones específicas por su ID.
     *
     * Propósito del componente:
     * Centralizar la lógica de acceso a datos de las relaciones permisos-roles,
     * asegurando integridad, consistencia y facilidad de mantenimiento.
     * Actúa como capa intermedia entre los servicios y la base de datos, promoviendo 
     * código limpio, desacoplado y confiable.
     */
public interface IPermRolRepositorio : IRepositorio<PermRol>
{
    /// Actualiza una relación PermRol existente y devuelve la entidad actualizada.
    Task<PermRol> ActualizarPermRol(PermRol entidad);
    /// Obtiene todas las relaciones PermRol con sus detalles completos.
    Task<List<PermRol>> ObtenerPermRolConDetalles();
    /// Obtiene una relación PermRol específica por su ID con detalles completos.
    Task<PermRol?> ObtenerPermRolConDetallesPorId(int id);

}

