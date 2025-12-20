using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * PagoRepositorio
     *
     * Repositorio especializado para la gestión de pagos asociados a órdenes dentro del sistema.
     *
     * Funcionalidades clave:
     * - Administración y actualización del estado de los pagos realizados por los usuarios.
     * - Obtención de información completa de cada pago con datos extendidos de la orden y del medio de pago.
     *
     * Propósito del componente:
     * Garantizar el manejo confiable y eficiente de los pagos efectuados dentro del proceso de compra,
     * permitiendo acceder a sus detalles financieros y relacionales.
     *
     * Descripción del código:
     * - Se extiende la funcionalidad del repositorio genérico para cubrir requisitos específicos de la entidad Pago.
     * - Se implementa una consulta optimizada que incluye relaciones necesarias para informes y monitoreo de pagos.
     */
public class PagoRepositorio : Repositorio<Pago>, IPagoRepositorio
{
    private readonly AppDbContext _db;// Contexto principal de persistencia de datos

    public PagoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza la información de un pago registrado en el sistema.
    public async Task<Pago> ActualizarPago(Pago entidad)
    {
        _db.Pagos.Update(entidad);         // Marca la entidad como modificada
        await _db.SaveChangesAsync();     // Persiste los cambios en la base de datos
        return entidad;                   // Retorna el pago actualizado
    }
    /// Recupera todos los pagos con información relacionada,
    /// incluyendo la orden y el tipo de medio de pago utilizado.
    public async Task<List<Pago>> ObtenerPagosConDetalles()
    {
        return await _db.Pagos
            .AsNoTracking()
            .Include(ur => ur.Orden)
            .Include(pt => pt.MedioPago)
            .ThenInclude(p => p.TipoPago)
            .ToListAsync();
    }
}
