using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * HistoriaRepositorio
     *
     * Repositorio especializado para la entidad Historia.
     * Funcionalidades clave:
     * - Permite gestionar la persistencia de historias utilizando Entity Framework Core.
     * - Proporciona métodos para actualizar registros de historia.
     *
     * Propósito del componente:
     * Facilitar el acceso y la actualización de la información de historias, 
     * asegurando consistencia y eficiencia en la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
     * - Método ActualizarHistoria: actualiza un registro de historia en la base de datos y persiste los cambios.
     */
public class HistoriaRepositorio : Repositorio<Historia>, IHistoriaRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public HistoriaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una historia existente en la base de datos
    public async Task<Historia> ActualizarHistoria(Historia entidad)
    {
        _db.Historias.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync(); // Persiste los cambios
        return entidad;               // Retorna la entidad actualizada
    }
}
