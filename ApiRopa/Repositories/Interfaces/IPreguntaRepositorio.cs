using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;

namespace ApiRopa;
/*
     * IPreguntaRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de la entidad Pregunta.
     * Funcionalidades clave:
     * - CRUD básico sobre Pregunta.
     * - Actualización de preguntas existentes mediante su entidad.
     *
     * Propósito del componente:
     * Centralizar y abstraer el acceso a datos de preguntas,
     * garantizando consistencia y facilidad de mantenimiento.
     * Sirve como capa intermedia entre servicios y la base de datos,
     * promoviendo un código limpio y desacoplado.
     */
public interface IPreguntaRepositorio: IRepositorio<Pregunta>
{
    /// Actualiza una entidad Pregunta existente y devuelve la entidad actualizada.
    Task<Pregunta> ActualizarPregunta(Pregunta entidad);
}
