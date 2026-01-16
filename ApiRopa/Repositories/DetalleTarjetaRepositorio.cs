using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * DetalleTarjetaRepositorio
     *
     * Repositorio especializado para la entidad DetalleTarjeta.
     * Funcionalidades clave:
     * - Gestiona la persistencia de DetalleTarjeta utilizando Entity Framework Core.
     * - Permite actualizar detalles de tarjetas y obtener listas con información relacionada.
     *
     * Propósito del componente:
     * Facilitar el acceso y la manipulación de los detalles de tarjetas asociadas a pagos,
     * asegurando consistencia y eficiencia en las operaciones de la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y la clase base genérica.
     * - Método ActualizarDetalleTarjeta: actualiza un detalle de tarjeta existente y persiste los cambios.
     * - Método ObtenerDetalleTarjetasConDetalles: obtiene todas las tarjetas incluyendo información de pago relacionada.
     */
public class DetalleTarjetaRepositorio : Repositorio<DetalleTarjeta>, IDetalleTarjetaRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public DetalleTarjetaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza un detalle de tarjeta existente en la base de datos
    public async Task<DetalleTarjeta> ActualizarDetalleTarjeta(DetalleTarjeta entidad)
    {
        _db.DetalleTarjetas.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync();       // Persiste los cambios
        return entidad;                     // Retorna la entidad actualizada
    }
    /// Obtiene todos los detalles de tarjetas incluyendo los pagos relacionados
}

