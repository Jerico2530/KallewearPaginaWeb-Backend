using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
/*
     * UserRolRepositorio
     *
     * Repositorio especializado en la administración de las relaciones entre usuarios y roles.
     *
     * Funcionalidades clave:
     * - Actualización de asignaciones de roles para usuarios.
     * - Obtención de UserRoles con la información detallada del usuario y su rol.
     *
     * Propósito del componente:
     * Encapsular la lógica de acceso a datos relacionada con la asignación de roles,
     * garantizando un manejo seguro y estructurado de la información sin exponer detalles de persistencia.
     *
     * Descripción del código:
     * - Extiende la funcionalidad genérica del repositorio base para casos específicos de UserRol.
     * - Utiliza EF Core para actualización y consultas con relaciones mediante Include().
     */

namespace ApiRopa.Repositorio
{
    public class UserRolRepositorio : Repositorio<UserRol>, IUserRolRepositorio
    {
        private readonly AppDbContext _db;// Contexto encargado de la conexión y persistencia de datos

        public UserRolRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza una asignación de rol existente para un usuario.
        public async Task<UserRol> ActualizarUserRol(UserRol entidad)
        {
            _db.UserRoles.Update(entidad);   // Marca la entidad como modificada para su persistencia
            await _db.SaveChangesAsync();  // Confirma los cambios en la base de datos
            return entidad;                // Retorna la entidad actualizada
        }
        /// Obtiene todos los registros UserRol incluyendo los detalles del usuario y su rol.
        public async Task<List<UserRol>> ObtenerUserRolesConDetalles()
        {
            return await _db.UserRoles
                .Include(ur => ur.Usuario)
                .Include(ur => ur.Rol)
                .ToListAsync();
        }
        /// Busca un UserRol específico por ID con detalles completos de sus relaciones.
        public async Task<UserRol?> ObtenerUserRolConDetallesPorId(int id)
        {
            return await _db.UserRoles
                .Include(ur => ur.Usuario)
                .Include(ur => ur.Rol)
                .FirstOrDefaultAsync(ur => ur.UserRolId == id);
        }

    }
}
