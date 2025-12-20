using BiblotecaWeb.Domain.Entities;
/*
    * IUsuarioRepositorio
    *
    * Interfaz de repositorio especializada para la entidad Usuario.
    * Funcionalidades clave:
    * - Hereda operaciones CRUD genéricas de IRepositorio<Usuario>.
    * - Permite actualizar registros de Usuario de manera controlada.
    *
    * Propósito del componente:
    * Brindar una capa de abstracción para acceder y manipular la entidad Usuario,
    * separando la lógica de negocio de la persistencia de datos y facilitando
    * pruebas, mantenimiento y consistencia.
    *
    * Descripción del código:
    * Define los métodos que deben implementarse para manejar la entidad Usuario,
    * incluyendo la actualización de registros en la base de datos.
    */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio : IRepositorio<Usuario>
    {
        /// Actualiza un registro de Usuario existente en la base de datos.
        Task<Usuario> ActualizarUsuario(Usuario entidad);

    }
}
