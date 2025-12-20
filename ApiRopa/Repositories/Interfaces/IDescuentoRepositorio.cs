using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
/*
     * IDescuentoRepositorio
     *
     * Interfaz de repositorio especializada en la gestión de descuentos aplicables a productos o servicios.
     * Funcionalidades clave:
     * - Operaciones CRUD básicas sobre entidades de tipo Descuento.
     * - Actualización de descuentos existentes con persistencia en base de datos.
     *
     * Propósito del componente:
     * Proporcionar una capa de abstracción para el acceso a datos de descuentos, asegurando que
     * la lógica de persistencia esté desacoplada de la lógica de negocio.
     * Facilita la mantenibilidad, consistencia y escalabilidad del proyecto.
     */
namespace ApiRopa;

public interface IDescuentoRepositorio : IRepositorio<Descuento>
{
    /// Actualiza un descuento existente y devuelve la entidad actualizada.
    Task<Descuento> ActualizarDescuento(Descuento entidad);
}
