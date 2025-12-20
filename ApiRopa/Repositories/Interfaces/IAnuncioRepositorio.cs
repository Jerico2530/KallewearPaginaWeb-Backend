using BiblotecaWeb.Domain.Entities;

namespace ApiRopa.Repositorio.IRepositorio
{
    /*
     * IAnuncioRepositorio
     *
     * Interfaz de repositorio especializada para la entidad Anuncio.
     * Funcionalidades clave:
     * - Extiende operaciones básicas de IRepositorio (CRUD genérico).
     * - Incluye operación específica para actualizar un anuncio y devolver la entidad actualizada.
     *
     * Propósito del componente:
     * Centralizar las operaciones de acceso a datos relacionadas con anuncios,
     * asegurando consistencia y encapsulación de la lógica de persistencia.
     *
     * Este componente actúa como capa de abstracción entre el servicio de negocio
     * y la base de datos, manteniendo el código desacoplado y fácil de mantener.
     */
    public interface IAnuncioRepositorio : IRepositorio<Anuncio>
    {

        /// Actualiza un anuncio existente y devuelve la entidad actualizada.
        Task<Anuncio> ActualizarAnuncio(Anuncio entidad);
    }
}
