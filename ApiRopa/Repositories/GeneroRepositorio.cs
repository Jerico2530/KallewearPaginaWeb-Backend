using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * GeneroRepositorio
     *
     * Repositorio especializado para la entidad Genero.
     * Funcionalidades clave:
     * - Permite gestionar la persistencia de géneros utilizando Entity Framework Core.
     * - Proporciona métodos para actualizar géneros y manipular datos relacionados.
     *
     * Propósito del componente:
     * Facilitar el acceso y la actualización de la información de géneros, 
     * asegurando consistencia y eficiencia en la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
     * - Método ActualizarGenero: actualiza un registro de género en la base de datos y persiste los cambios.
     */
public class GeneroRepositorio : Repositorio<Genero>, IGeneroRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public GeneroRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un género existente en la base de datos
    public async Task<Genero> ActualizarGenero(Genero entidad)
    {
        _db.Generos.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync(); // Persiste los cambios
        return entidad;               // Retorna la entidad actualizada
    }
}
