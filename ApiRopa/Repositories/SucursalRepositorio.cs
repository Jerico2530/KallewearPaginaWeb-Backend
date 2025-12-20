using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
     * SucursalRepositorio
     *
     * Repositorio especializado para la administración de sucursales dentro del sistema.
     *
     * Funcionalidades clave:
     * - Actualización de datos de sucursales ya registradas.
     * - Manejo centralizado de la persistencia de información asociada a la entidad Sucursal.
     *
     * Propósito del componente:
     * Garantizar la correcta gestión de la información de sucursales de la organización,
     * permitiendo modificar sus datos sin exponer directamente la lógica de acceso a base de datos.
     *
     * Descripción del código:
     * - Extiende un repositorio genérico para reutilizar operaciones CRUD comunes.
     * - Implementa una función específica de actualización utilizando Entity Framework Core.
     */
public class SucursalRepositorio : Repositorio<Sucursal>, ISucursalRepositorio
{
    private readonly AppDbContext _db; // Contexto principal de persistencia de datos

    public SucursalRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una sucursal existente en la base de datos.
    public async Task<Sucursal> ActualizarSucursal(Sucursal entidad)
    {
        _db.Sucursales.Update(entidad);     // Marca el registro como modificado para su posterior persistencia
        await _db.SaveChangesAsync();     // Guarda los cambios realizados en la base de datos
        return entidad;                   // Devuelve la entidad actualizada
    }
}
