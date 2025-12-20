using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
/*
     * AnuncioRepositorio
     *
     * Implementación concreta del repositorio para la entidad Anuncio.
     * Funcionalidades clave:
     * - Hereda operaciones CRUD genéricas de Repositorio<Anuncio>.
     * - Permite actualizar anuncios específicos en la base de datos.
     *
     * Propósito del componente:
     * Gestionar la persistencia de datos de los anuncios, proporcionando
     * un acceso estructurado y consistente a la base de datos mediante EF Core.
     *
     * Descripción del código:
     * - Constructor: recibe el contexto de base de datos e inicializa la clase base.
     * - ActualizarAnuncio: actualiza un anuncio existente y guarda los cambios en la base de datos.
     */
namespace ApiRopa.Repositorio
{
    public class AnuncioRepositorio : Repositorio<Anuncio>, IAnuncioRepositorio
    {
        private readonly AppDbContext _db;// Contexto de base de datos para acceso a tablas

        public AnuncioRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un registro de Anuncio en la base de datos.
        public async Task<Anuncio> ActualizarAnuncio(Anuncio entidad)
        {
            _db.Anuncios.Update(entidad); // Marca la entidad como modificada
            await _db.SaveChangesAsync(); // Guarda los cambios de manera asincrónica
            return entidad; // Retorna la entidad actualizada
        }
    }
}
