using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * CategoriaRepositorio
     *
     * Repositorio especializado para la entidad Categoria.
     * Funcionalidades clave:
     * - Gestiona la persistencia de Categorias utilizando Entity Framework Core.
     * - Permite operaciones CRUD básicas, incluyendo actualización de entidades existentes.
     *
     * Propósito del componente:
     * Facilitar el acceso y la manipulación de datos de categorías dentro del sistema,
     * asegurando consistencia y eficiencia en las operaciones de la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y la clase base genérica.
     * - Método ActualizarCategoria: actualiza una categoría existente y persiste los cambios.
     */
public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public CategoriaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una categoría existente en la base de datos
    public async Task<Categoria> ActualizarCategoria(Categoria entidad)
    {
        _db.Categorias.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync();  // Persiste los cambios en la base de datos
        return entidad;                // Retorna la entidad actualizada
    }
}

