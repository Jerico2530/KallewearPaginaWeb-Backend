using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * NoticiaRepositorio
     *
     * Repositorio especializado para la entidad Noticia.
     *
     * Funcionalidades clave:
     * - Gestiona la persistencia de noticias utilizando Entity Framework Core.
     * - Permite actualizar registros existentes de noticias.
     *
     * Propósito del componente:
     * Facilitar la gestión eficiente y consistente de los datos de noticias dentro de la aplicación.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
     * - Método ActualizarNoticia: actualiza un registro de noticia existente y persiste los cambios.
     */
public class NoticiaRepositorio : Repositorio<Noticia>, INoticiaRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public NoticiaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una noticia existente en la base de datos
    public async Task<Noticia> ActualizarNoticia(Noticia entidad)
    {
        _db.Noticias.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync(); // Persiste los cambios en la base de datos
        return entidad; // Retorna la entidad actualizada
    }
}
