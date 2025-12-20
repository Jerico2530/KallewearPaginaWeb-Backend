using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model.Dto;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using BiblotecaWeb.Domain.Entities;
/*
     * UsuarioRepositorio
     *
     * Repositorio especializado para la administración y persistencia de datos de usuarios.
     *
     * Funcionalidades clave:
     * - Actualización de información de usuarios existentes.
     * - Encapsulamiento de operaciones de acceso a datos para la entidad Usuario.
     *
     * Propósito del componente:
     * Garantizar un manejo estructurado y seguro de la información del usuario,
     * aislando los detalles de la base de datos de la lógica de negocio.
     *
     * Descripción del código:
     * - Aprovecha la funcionalidad genérica del repositorio base.
     * - Implementa una operación específica para modificar registros de usuarios mediante EF Core.
     */
namespace ApiRopa.Repositorio
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly AppDbContext _db;// Contexto de acceso y persistencia a la base de datos

        public UsuarioRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un usuario existente en la base de datos.
        public async Task<Usuario> ActualizarUsuario(Usuario entidad)
        {
            _db.Usuarios.Update(entidad);   // Marca los cambios de la entidad para su persistencia
            await _db.SaveChangesAsync();  // Ejecuta y confirma la actualización en la base de datos
            return entidad;                // Retorna la entidad ya actualizada
        }



       
    }
}
