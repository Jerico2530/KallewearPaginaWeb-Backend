using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;
/*
     * Repositorio<T>
     *
     * Componente base del patrón Repository que administra operaciones CRUD genéricas
     * sobre cualquier entidad del sistema.
     *
     * Funcionalidades clave:
     * - Creación, obtención, actualización y eliminación de entidades.
     * - Soporte para filtros dinámicos, consultas con tracking opcional y carga de relaciones.
     *
     * Propósito del componente:
     * Centralizar la lógica de acceso a datos reutilizable para todas las entidades,
     * garantizando mantenibilidad, cohesión y reducción de duplicación de código en la capa de persistencia.
     *
     * Descripción del código:
     * - Utiliza EF Core para ejecutar operaciones asíncronas contra la base de datos.
     * - Expone métodos genéricos que pueden ser extendidos por repositorios específicos.
     */
namespace ApiRopa.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly AppDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public async Task Crear(T entidad)
        {
            await dbSet.AddAsync(entidad); 
            await Grabar();
        }
        public async Task Grabar()
        {
            await _db.SaveChangesAsync();
        }

        public async Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true , Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (filtro != null)
            {
                query = query.Where(filtro);

            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> ObtenerTodo(Expression<Func<T, bool>>? filtro = null)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
            {
                query = query.Where(filtro);

            }
            return await query.ToListAsync();

        }

        public async Task Remover(T entidad)
        {
            dbSet.Remove(entidad);
            await Grabar();
        }

        public async Task<bool> Existe(Expression<Func<T, bool>> filtro)
        {
            return await dbSet.AnyAsync(filtro);
        }
        public async Task ActualizarVariosAsync(IEnumerable<T> entidades)
        {
            dbSet.UpdateRange(entidades);
            await _db.SaveChangesAsync();
        }

    }
}
