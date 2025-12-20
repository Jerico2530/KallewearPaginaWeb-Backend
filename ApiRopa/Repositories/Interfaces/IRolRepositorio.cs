using BiblotecaWeb.Domain.Entities;
/*
    * IRolRepositorio
    *
    * Interfaz de repositorio especializada para la entidad Rol.
    * Funcionalidades clave:
    * - Hereda operaciones CRUD genéricas de IRepositorio<Rol>.
    * - Permite actualizar roles existentes en la base de datos.
    *
    * Propósito del componente:
    * Centralizar el acceso y la manipulación de datos de roles,
    * garantizando consistencia, mantenibilidad y una capa de abstracción limpia
    * sobre la base de datos.
    *
    * Descripción del código:
    * Define los métodos que deben implementarse para gestionar la entidad Rol,
    * incluyendo la actualización específica de un rol existente.
    */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface IRolRepositorio : IRepositorio<Rol>
    {
        /// Actualiza una entidad Rol existente en la base de datos.
        Task<Rol> ActualizarRol(Rol entidad);
    }
}
