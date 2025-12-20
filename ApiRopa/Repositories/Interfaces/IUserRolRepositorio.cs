using BiblotecaWeb.Domain.Entities;
/*
     * IUserRolRepositorio
     *
     * Interfaz de repositorio especializada para la entidad UserRol.
     * Funcionalidades clave:
     * - Hereda operaciones CRUD genéricas de IRepositorio<UserRol>.
     * - Permite actualizar registros de UserRol.
     * - Permite obtener listas y detalles específicos de UserRol.
     *
     * Propósito del componente:
     * Proporcionar una capa de abstracción para el acceso y manejo de la entidad UserRol,
     * asegurando separación entre la lógica de negocio y la persistencia de datos,
     * facilitando pruebas, mantenimiento y consistencia.
     *
     * Descripción del código:
     * Define los métodos que deben implementarse para manipular la entidad UserRol,
     * incluyendo actualización y obtención de datos con detalles asociados.
     */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface IUserRolRepositorio : IRepositorio<UserRol>
    {
        /// Actualiza un registro de UserRol existente en la base de datos.
        Task<UserRol> ActualizarUserRol(UserRol entidad);
        /// Obtiene todos los registros de UserRol incluyendo detalles relacionados.
        Task<List<UserRol>> ObtenerUserRolesConDetalles();
        /// Obtiene un registro de UserRol específico con detalles asociados por su ID.
        Task<UserRol?> ObtenerUserRolConDetallesPorId(int id);

    }
}
