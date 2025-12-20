using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
    * OrdenDetalleRepositorio
    *
    * Repositorio especializado para la entidad OrdenDetalle.
    *
    * Funcionalidades clave:
    * - Gestiona la persistencia de los detalles de orden utilizando Entity Framework Core.
    * - Permite actualizar registros existentes de detalles de orden.
    *
    * Propósito del componente:
    * Facilitar la gestión eficiente y consistente de los detalles de orden dentro del sistema.
    *
    * Descripción del código:
    * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
    * - Método ActualizarOrdenDetalle: actualiza un registro de detalle de orden existente y persiste los cambios.
    */
public class OrdenDetalleRepositorio : Repositorio<OrdenDetalle>, IOrdenDetalleRepositorio
{
    private readonly AppDbContext _db;  // Contexto EF Core para acceso a la base de datos

    public OrdenDetalleRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un detalle de orden existente en la base de datos
    public async Task<OrdenDetalle> ActualizarOrdenDetalle(OrdenDetalle entidad)
    {
        _db.OrdenDetalles.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync(); // Persiste los cambios en la base de datos
        return entidad; // Retorna la entidad actualizada
    }
}
