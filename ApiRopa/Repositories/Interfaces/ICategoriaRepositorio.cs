using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
     * ICategoriaRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de categorías de productos.
     * Funcionalidades clave:
     * - CRUD básico para entidades de tipo Categoria.
     * - Actualización de categorías existentes.
     *
     * Propósito del componente:
     * Proveer una capa de abstracción para todas las operaciones de acceso a datos relacionadas
     * con categorías, asegurando consistencia y mantenibilidad del código.
     * Actúa como intermediario entre la base de datos y los servicios de negocio, manteniendo
     * la lógica de persistencia desacoplada y profesional.
     */
namespace ApiRopa;

public interface ICategoriaRepositorio : IRepositorio<Categoria>
{
    /// Actualiza una categoría existente y devuelve la entidad actualizada.
    Task<Categoria> ActualizarCategoria(Categoria entidad);
}

