using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
/*
    * RolRepositorio
    *
    * Repositorio especializado para la gestión de roles dentro de la plataforma.
    *
    * Funcionalidades clave:
    * - Actualización de registros de roles existentes.
    * - Manejo de operaciones de persistencia asociadas a entidades Rol.
    *
    * Propósito del componente:
    * Proporcionar una capa de acceso a datos específica para roles,
    * permitiendo modificar la configuración del sistema sin exponer los detalles
    * internos de Entity Framework Core ni duplicar lógica de persistencia.
    *
    * Descripción del código:
    * - Extiende una implementación genérica de repositorio para reutilizar operaciones comunes.
    * - Implementa una función concreta para actualizar entidades Rol en la base de datos.
    */
namespace ApiRopa.Repositorio
{
    public class RolRepositorio : Repositorio<Rol>, IRolRepositorio
    {
        private readonly AppDbContext _db;// Contexto de acceso a la base de datos

        public RolRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un rol existente en la base de datos.
        public async Task<Rol> ActualizarRol(Rol entidad)
        {
            _db.Roles.Update(entidad);        // Registra la entidad como modificada para su actualización
            await _db.SaveChangesAsync();   // Persiste los cambios en la base de datos
            return entidad;                 // Retorna la entidad ya actualizada
        }
    }
}
