using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa;
/*
     * ISucursalRepositorio
     *
     * Interfaz de repositorio especializada para la entidad Sucursal.
     * Funcionalidades clave:
     * - Hereda operaciones CRUD genéricas de IRepositorio<Sucursal>.
     * - Permite actualizar sucursales existentes en la base de datos.
     *
     * Propósito del componente:
     * Centralizar el acceso y la manipulación de datos de sucursales,
     * asegurando consistencia y mantenibilidad, ofreciendo una capa de abstracción
     * clara entre la lógica de negocio y la base de datos.
     *
     * Descripción del código:
     * Define los métodos que deben implementarse para gestionar la entidad Sucursal,
     * incluyendo la actualización específica de una sucursal existente.
     */
public interface ISucursalRepositorio : IRepositorio<Sucursal>
{
    /// Actualiza una entidad Sucursal existente en la base de datos.
    Task<Sucursal> ActualizarSucursal(Sucursal entidad);
}
