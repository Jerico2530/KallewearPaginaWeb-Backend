using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb;
using ApiRopa.Repositorio.IRepositorio;

namespace ApiRopa;
/*
     * PermisoRepositorio
     *
     * Repositorio especializado para la administración de permisos del sistema.
     *
     * Funcionalidades clave:
     * - Gestión y actualización de los permisos asignados a los usuarios.
     * - Abstracción de la lógica de acceso a datos mediante el patrón Repository.
     *
     * Propósito del componente:
     * Centralizar la persistencia y las operaciones relacionadas con la entidad Permiso,
     * asegurando un mantenimiento claro y escalable de la seguridad y roles de usuario.
     *
     * Descripción del código:
     * - Extiende el repositorio genérico para cubrir requisitos específicos de permisos.
     * - Expone un método para actualizar la información de un permiso dentro del sistema.
     */
public class PermisoRepositorio : Repositorio<Permiso>, IPermisoRepositorio
{
    private readonly AppDbContext _db; // Contexto principal de persistencia de datos

    public PermisoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un permiso existente en la base de datos
    public async Task<Permiso> ActualizarPermiso(Permiso entidad)
    {
        _db.Permisos.Update(entidad);     // Marca la entidad como modificada para persistir cambios
        await _db.SaveChangesAsync();    // Guarda los cambios en la base de datos
        return entidad;                  // Retorna la entidad actualizada
    }
}
