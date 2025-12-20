using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
/*
     * IHistoriaRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de historias dentro del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre la entidad Historia.
     * - Actualización de registros existentes.
     *
     * Propósito del componente:
     * Proporcionar una capa de abstracción para el acceso a datos de historias, 
     * asegurando consistencia y centralización de la lógica de persistencia.
     * Actúa como intermediario entre la lógica de negocio y la base de datos.
     */
namespace ApiRopa;

public interface IHistoriaRepositorio : IRepositorio<Historia>
{
    /// Actualiza una historia existente y devuelve la entidad actualizada.
    Task<Historia> ActualizarHistoria(Historia entidad);
}
