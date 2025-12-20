using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb;

namespace ApiRopa;
/*
   * TipoPagoRepositorio
   *
   * Repositorio especializado para la gestión de métodos de pago dentro del sistema.
   *
   * Funcionalidades clave:
   * - Actualización de registros existentes de tipos de pago.
   * - Administración centralizada de operaciones de persistencia relacionadas a pagos.
   *
   * Propósito del componente:
   * Mantener de manera eficiente la información referente a los métodos de pago,
   * permitiendo su modificación y control desde la capa de datos sin exponer la 
   * implementación interna de acceso a la base de datos.
   *
   * Descripción del código:
   * - Hereda operaciones CRUD genéricas desde Repositorio<TipoPago>.
   * - Implementa una actualización directa de entidades TipoPago.
   * - Persiste los cambios mediante Entity Framework Core.
   */
public class TipoPagoRepositorio : Repositorio<TipoPago>, ITipoPagoRepositorio
{
    private readonly AppDbContext _db; // Contexto principal para acceso a datos

    public TipoPagoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un registro de tipo de pago existente en la base de datos.
    public async Task<TipoPago> ActualizarTipoPago(TipoPago entidad)
    {
        _db.TipoPagos.Update(entidad);  // Se marca la entidad como modificada
        await _db.SaveChangesAsync();  // Se guardan los cambios aplicados
        return entidad;                // Devuelve el objeto actualizado
    }
}

