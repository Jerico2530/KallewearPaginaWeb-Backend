using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * MedioPagoRepositorio
     *
     * Repositorio especializado para la entidad MedioPago.
     * Funcionalidades clave:
     * - Gestiona la persistencia de medios de pago utilizando Entity Framework Core.
     * - Permite actualizar registros existentes y obtener medios de pago con sus detalles relacionados.
     *
     * Propósito del componente:
     * Facilitar el acceso y la gestión de la información de medios de pago, 
     * asegurando eficiencia y consistencia en la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y hereda funcionalidades del repositorio genérico.
     * - Método ActualizarMedioPago: actualiza un registro de medio de pago y persiste los cambios.
     * - Método ObtenerMedioPagosConDetalles: obtiene todos los medios de pago incluyendo los detalles del tipo de pago asociado.
     */
public class MedioPagoRepositorio : Repositorio<MedioPago>, IMedioPagoRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public MedioPagoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un medio de pago existente en la base de datos
    public async Task<MedioPago> ActualizarMedioPago(MedioPago entidad)
    {
        _db.MedioPagos.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync();   // Persiste los cambios
        return entidad;                 // Retorna la entidad actualizada
    }
    /// Obtiene todos los medios de pago incluyendo los detalles del tipo de pago
    public async Task<List<MedioPago>> ObtenerMedioPagosConDetalles()
    {
        return await _db.MedioPagos
             .Include(ur => ur.TipoPago) // Incluye información relacionada del tipo de pago
                .ToListAsync();             // Devuelve la lista completa
    }
}
