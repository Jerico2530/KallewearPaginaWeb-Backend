using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
    * INoticiaRepositorio
    *
    * Interfaz de repositorio especializada en la gestión de noticias dentro del sistema.
    * Funcionalidades clave:
    * - Operaciones CRUD básicas sobre la entidad Noticia.
    * - Actualización de registros existentes.
    *
    * Propósito del componente:
    * Abstraer y centralizar el acceso a datos de noticias, asegurando consistencia
    * y facilitando la reutilización en la lógica de negocio. Sirve como capa intermedia
    * entre los servicios de negocio y la base de datos.
    */
namespace ApiRopa;

public interface INoticiaRepositorio : IRepositorio<Noticia>
{
    /// Actualiza una noticia existente y devuelve la entidad actualizada.
    Task<Noticia> ActualizarNoticia(Noticia entidad);
}
