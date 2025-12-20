using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;
/*
     * IMonedaRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de monedas dentro del sistema.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre la entidad Moneda.
     * - Actualización de registros existentes.
     *
     * Propósito del componente:
     * Centralizar y abstraer el acceso a datos de monedas, garantizando consistencia
     * y reutilización en la lógica de negocio. Actúa como capa intermedia entre
     * los servicios de negocio y la base de datos.
     */
namespace ApiRopa;

public interface IMonedaRepositorio : IRepositorio<Moneda>
{
    /// Actualiza una moneda existente y devuelve la entidad actualizada.
    Task<Moneda> ActualizarMoneda(Moneda entidad);
}
