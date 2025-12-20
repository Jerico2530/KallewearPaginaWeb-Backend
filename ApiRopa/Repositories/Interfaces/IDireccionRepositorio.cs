using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;
/*
     * IDireccionRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de direcciones de usuarios o entidades relacionadas.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre la entidad Direccion.
     * - Actualización de direcciones existentes.
     * - Obtención de listas de direcciones incluyendo información relacionada completa.
     *
     * Propósito del componente:
     * Centralizar y desacoplar el acceso a datos de direcciones, asegurando consistencia e integridad
     * de la información en toda la aplicación.
     * Actúa como capa intermedia entre la lógica de negocio y la persistencia de datos.
     */
namespace ApiRopa;

public interface IDireccionRepositorio : IRepositorio<Direccion>
{
    /// Actualiza una dirección existente y devuelve la entidad actualizada.
    Task<Direccion> ActualizarDireccion(Direccion entidad);
    /// Obtiene la lista de direcciones incluyendo sus relaciones completas.
    Task<List<Direccion>> ObtenerDetalleDireccionesConDetalles();
}

