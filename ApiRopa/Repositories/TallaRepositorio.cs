using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * TallaRepositorio
     *
     * Repositorio especializado para la administración de tallas dentro del sistema.
     *
     * Funcionalidades clave:
     * - Actualización de registros de tallas existentes.
     * - Gestión eficiente y centralizada de la persistencia de datos relacionados a tallas.
     *
     * Propósito del componente:
     * Mantener la información de tallas de manera ordenada y escalable,
     * asegurando que cualquier modificación a una talla se realice mediante una capa de acceso a datos controlada.
     *
     * Descripción del código:
     * - Hereda del repositorio genérico para reusar operaciones comunes de CRUD.
     * - Implementa una operación específica de actualización sobre la entidad Talla.
     * - Utiliza Entity Framework Core para persistir los cambios en la base de datos.
     */
public class TallaRepositorio : Repositorio<Talla>, ITallaRepositorio
{
    private readonly AppDbContext _db; // Contexto principal de interacción con la base de datos

    public TallaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una talla existente en la base de datos.
    public async Task<Talla> ActualizarTalla(Talla entidad)
    {
        _db.Tallas.Update(entidad);     // Indica a EF Core que la entidad ha cambiado
        await _db.SaveChangesAsync();  // Persiste los cambios realizados
        return entidad;                // Devuelve la entidad modificada
    }
}

