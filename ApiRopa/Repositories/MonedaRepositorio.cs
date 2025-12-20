using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb;

namespace ApiRopa;
/*
     * MonedaRepositorio
     *
     * Repositorio especializado para la entidad Moneda.
     *
     * Funcionalidades clave:
     * - Gestiona la persistencia de monedas utilizando Entity Framework Core.
     * - Permite actualizar registros existentes en la base de datos.
     *
     * Propósito del componente:
     * Facilitar la gestión eficiente y consistente de los datos de monedas en la aplicación.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
     * - Método ActualizarMoneda: actualiza un registro de moneda existente y persiste los cambios.
     */
public class MonedaRepositorio : Repositorio<Moneda>, IMonedaRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public MonedaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una moneda existente en la base de datos
    public async Task<Moneda> ActualizarMoneda(Moneda entidad)
    {
        _db.Monedas.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync(); // Persiste los cambios en la base de datos
        return entidad; // Retorna la entidad actualizada
    }
}
