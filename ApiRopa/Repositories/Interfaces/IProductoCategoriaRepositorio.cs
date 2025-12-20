using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;
/*
    * IProductoCategoriaRepositorio
    *
    * Interfaz de repositorio especializada en la gestión de la entidad ProductoCategoria.
    * Funcionalidades clave:
    * - CRUD básico sobre ProductoCategoria.
    * - Obtención de listas completas con detalles relacionados.
    * - Recuperación de un ProductoCategoria específico con todos sus detalles.
    *
    * Propósito del componente:
    * Abstraer y centralizar el acceso a datos de ProductoCategoria,
    * garantizando consistencia y facilitando la mantenibilidad.
    * Actúa como capa intermedia entre los servicios de negocio y la base de datos,
    * asegurando operaciones claras y desacopladas.
    */
public interface IProductoCategoriaRepositorio : IRepositorio<ProductoCategoria>
{
    /// Actualiza una entidad ProductoCategoria existente y devuelve la entidad actualizada.
    Task<ProductoCategoria> ActualizarProductoCategoria(ProductoCategoria entidad);
    /// Obtiene todas las entidades ProductoCategoria con sus relaciones y detalles completos.
    Task<List<ProductoCategoria>> ObtenerProductoCategoriaConDetalles();
    /// Obtiene un ProductoCategoria específico junto con sus detalles por su ID.
    Task<ProductoCategoria?> ObtenerProductoCategoriaConDetallesPorId(int id);

}
