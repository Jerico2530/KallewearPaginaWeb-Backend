using BiblotecaWeb.Domain.Entities;
/*
     * ITestimonioRepositorio
     *
     * Interfaz de repositorio especializada para la entidad Testimonio.
     * Funcionalidades clave:
     * - Hereda operaciones CRUD genéricas de IRepositorio<Testimonio>.
     * - Permite actualizar testimonios existentes en la base de datos.
     * - Proporciona acceso a la lista de testimonios incluyendo detalles relacionados.
     *
     * Propósito del componente:
     * Centralizar el manejo de datos de testimonios, ofreciendo una capa de abstracción
     * que separa la lógica de negocio de la persistencia y facilita mantenimiento y pruebas.
     *
     * Descripción del código:
     * Define los métodos que deben implementarse para manipular la entidad Testimonio,
     * incluyendo actualización de registros y obtención de testimonios con información detallada.
     */
namespace ApiRopa.Repositorio.IRepositorio
{
    public interface ITestimonioRepositorio : IRepositorio<Testimonio>
    {
        /// Actualiza un Testimonio existente en la base de datos.
        Task<Testimonio> ActualizarTestimonio(Testimonio entidad);
        /// Obtiene la lista de Testimonios con sus detalles relacionados.
        Task<List<Testimonio>> ObtenerTestimoniosConDetalles();
    }
}
