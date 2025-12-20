using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
/*
     * IGeneroRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de géneros de productos o entidades relacionadas.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre la entidad Genero.
     * - Actualización de géneros existentes.
     *
     * Propósito del componente:
     * Centralizar y abstraer el acceso a datos de géneros, asegurando consistencia y facilidad de mantenimiento.
     * Actúa como capa intermedia entre la lógica de negocio y la persistencia de datos.
     */
namespace ApiRopa;

public interface IGeneroRepositorio : IRepositorio<Genero>
{
    /// Actualiza un género existente y devuelve la entidad actualizada.
    Task<Genero> ActualizarGenero(Genero entidad);
}
