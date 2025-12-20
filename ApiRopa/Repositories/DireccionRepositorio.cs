using BiblotecaWeb.Datos;
using BiblotecaWeb;
using ApiRopa.Repositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiRopa;
/*
     * DireccionRepositorio
     *
     * Repositorio especializado para la entidad Direccion.
     * Funcionalidades clave:
     * - Gestiona la persistencia de direcciones utilizando Entity Framework Core.
     * - Permite actualizar direcciones y obtener listas con información de usuario relacionada.
     *
     * Propósito del componente:
     * Facilitar el acceso y la manipulación de las direcciones de los usuarios,
     * asegurando consistencia y eficiencia en las operaciones de la capa de datos.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y la clase base genérica.
     * - Método ActualizarDireccion: actualiza una dirección existente y persiste los cambios.
     * - Método ObtenerDetalleDireccionesConDetalles: obtiene todas las direcciones incluyendo la relación con el usuario.
     */
public class DireccionRepositorio: Repositorio<Direccion>, IDireccionRepositorio
{
    private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

    public DireccionRepositorio(AppDbContext db) : base(db)
{
    _db = db;
}
    /// Actualiza una dirección existente en la base de datos
    public async Task<Direccion> ActualizarDireccion(Direccion entidad)
    {
        _db.Direcciones.Update(entidad); // Marca la entidad como modificada
        await _db.SaveChangesAsync();  // Persiste los cambios
        return entidad;                // Retorna la entidad actualizada
    }
    /// Obtiene todas las direcciones incluyendo la información de usuario relacionada
    public async Task<List<Direccion>> ObtenerDetalleDireccionesConDetalles()
    {
        return await _db.Direcciones
            .Include(ur => ur.Usuario) // Incluye la relación con la entidad Usuario
                .ToListAsync();            // Convierte a lista y devuelve
    }
}
