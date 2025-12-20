using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa;

/*
 * ITallaRepositorio
 *
 * Interfaz de repositorio especializada para la entidad Talla.
 * Funcionalidades clave:
 * - Hereda operaciones CRUD genéricas de IRepositorio<Talla>.
 * - Permite actualizar registros de talla existentes en la base de datos.
 *
 * Propósito del componente:
 * Centralizar el acceso y la gestión de datos de tallas, asegurando consistencia
 * y mantenibilidad, y ofreciendo una capa de abstracción clara entre la lógica
 * de negocio y la base de datos.
 *
 * Descripción del código:
 * Define los métodos que deben implementarse para manejar la entidad Talla,
 * incluyendo la actualización específica de un registro existente.
 */
public interface ITallaRepositorio : IRepositorio<Talla>
{
    /// Actualiza una entidad Talla existente en la base de datos.
    Task<Talla> ActualizarTalla(Talla entidad);
}

