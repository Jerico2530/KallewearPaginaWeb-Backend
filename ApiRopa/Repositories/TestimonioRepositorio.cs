using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
/*
     * TestimonioRepositorio
     *
     * Repositorio especializado para la gestión de testimonios de usuarios dentro del sistema.
     *
     * Funcionalidades clave:
     * - Actualización de testimonios existentes.
     * - Obtención de testimonios incluyendo información detallada del usuario asociado.
     * - Administración centralizada de la persistencia de datos relacionados a testimonios.
     *
     * Propósito del componente:
     * Brindar un punto de acceso seguro y estructurado para la modificación y consulta
     * de testimonios, manteniendo la consistencia en el acceso a datos mediante operaciones
     * encapsuladas en la capa repositorio.
     *
     * Descripción del código:
     * - Hereda funciones genéricas de CRUD desde Repositorio<T>.
     * - Implementa una actualización específica sobre la entidad Testimonio.
     * - Utiliza Entity Framework Core para la carga de relaciones y persistencia de datos.
     */
namespace ApiRopa.Repositorio
{
    public class TestimonioRepositorio : Repositorio<Testimonio>, ITestimonioRepositorio
    {
        private readonly AppDbContext _db; // Contexto utilizado para la interacción con la base de datos

        public TestimonioRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un testimonio registrado en la base de datos.
        public async Task<Testimonio> ActualizarTestimonio(Testimonio entidad)
        {
            _db.Testimonios.Update(entidad);     // Marca el registro como modificado
            await _db.SaveChangesAsync();      // Persiste los cambios aplicados
            return entidad;                    // Devuelve la entidad con las modificaciones
        }
        /// Obtiene la lista de testimonios incluyendo los datos de su usuario asociado.
        public async Task<List<Testimonio>> ObtenerTestimoniosConDetalles()
        {
            return await _db.Testimonios
                .Include(ur => ur.Usuario)
                .ToListAsync();
        }
    }
}
