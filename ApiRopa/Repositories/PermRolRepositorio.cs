using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;
using Microsoft.EntityFrameworkCore;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
    * PermRolRepositorio
    *
    * Repositorio especializado para la administración de la relación entre Permisos y Roles.
    *
    * Funcionalidades clave:
    * - Actualización de asignaciones entre permisos y roles.
    * - Obtención de los permisos por rol con sus relaciones asociadas.
    *
    * Propósito del componente:
    * Gestionar de forma centralizada el acceso y mantenimiento de la seguridad basada en roles,
    * permitiendo una administración escalable y controlada de los permisos otorgados en el sistema.
    *
    * Descripción del código:
    * - Implementa operaciones específicas del vínculo Permiso-Rol.
    * - Utiliza Entity Framework Core para incluir datos relacionados y optimizar consultas.
    */
public class PermRolRepositorio : Repositorio<PermRol>, IPermRolRepositorio
{
    private readonly AppDbContext _db;  // Contexto principal de persistencia de datos

    public PermRolRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una relación Permiso-Rol existente.
    public async Task<PermRol> ActualizarPermRol(PermRol entidad)
    {
        _db.PermRoles.Update(entidad);  // Marca la entidad como modificada para persistir cambios
        await _db.SaveChangesAsync(); // Guarda los cambios en la base de datos
        return entidad;               // Devuelve la entidad actualizada
    }
    /// Obtiene todas las relaciones Permiso-Rol con los datos asociados de cada tabla.
    public async Task<List<PermRol>> ObtenerPermRolConDetalles()
    {
        return await _db.PermRoles
            .Include(ur => ur.Permiso)
            .Include(ur => ur.Rol)
            .ToListAsync();
    }
    /// Obtiene una relación Permiso-Rol específica por Id con sus detalles asociados.
    public async Task<PermRol?> ObtenerPermRolConDetallesPorId(int id )
    {
        return await _db.PermRoles
            .Include(ur => ur.Permiso)
            .Include(ur => ur.Rol)
            .FirstOrDefaultAsync(ur => ur.PermRolId == id);
    }

}

