using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb;

namespace ApiRopa;
/*
     * ITipoPagoRepositorio
     *
     * Interfaz de repositorio especializada para la entidad TipoPago.
     * Funcionalidades clave:
     * - Hereda operaciones CRUD genéricas de IRepositorio<TipoPago>.
     * - Permite actualizar registros de TipoPago en la base de datos.
     *
     * Propósito del componente:
     * Proveer una capa de abstracción para el manejo de la entidad TipoPago,
     * separando la lógica de acceso a datos de la lógica de negocio y facilitando
     * mantenimiento, pruebas y consistencia en el manejo de pagos.
     *
     * Descripción del código:
     * Define los métodos que deben implementarse para manipular la entidad TipoPago,
     * específicamente la actualización de registros.
     */
public interface ITipoPagoRepositorio : IRepositorio<TipoPago>
{
    /// Actualiza un registro de TipoPago existente en la base de datos.
    Task<TipoPago> ActualizarTipoPago(TipoPago entidad);
}
