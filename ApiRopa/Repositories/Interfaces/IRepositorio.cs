using System.Linq.Expressions;
/*
     * IRepositorio<T>
     *
     * Interfaz genérica de repositorio para operaciones CRUD sobre cualquier entidad.
     * Funcionalidades clave:
     * - Crear, obtener, actualizar y eliminar entidades genéricas.
     * - Filtrado y seguimiento opcional de entidades.
     * - Soporte para inclusión de relaciones mediante expresiones LINQ.
     * - Operaciones de persistencia centralizadas.
     *
     * Propósito del componente:
     * Proporcionar una capa de abstracción uniforme sobre la base de datos,
     * promoviendo reutilización, mantenibilidad y consistencia en todas las entidades.
     * Sirve como base para repositorios especializados, reduciendo código duplicado
     * y asegurando prácticas de acceso a datos limpias y profesionales.
     */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        /// Crea una nueva entidad en el repositorio.
        Task Crear(T entidad);
        /// Obtiene todas las entidades que cumplan un filtro opcional.
        Task<List<T>> ObtenerTodo(Expression<Func<T, bool>>? filtro = null);
        /// Obtiene una entidad específica según el filtro, con opción de tracking y carga de relaciones.
        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true , Func<IQueryable<T>, IQueryable<T>> include = null);
        /// Verifica si existe alguna entidad que cumpla con el filtro.
        Task<bool> Existe(Expression<Func<T, bool>> filtro);
        /// Actualiza varias entidades en una operación.
        Task ActualizarVariosAsync(IEnumerable<T> entidades);
        /// Elimina una entidad del repositorio.
        Task Remover(T entidad);
        /// Persiste los cambios pendientes en la base de datos.
        Task Grabar();
    }
}
