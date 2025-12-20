using BiblotecaWeb.Domain.Entities;
/*
 * CarritoServicioDominio
 *
 * Componente de dominio encargado de gestionar la lógica de negocio relacionada con el carrito de compras.
 * Funcionalidades clave:
 * - Calcular subtotales de cada ítem del carrito.
 * - Calcular el total general del carrito sumando todos los subtotales.
 *
 * Propósito del componente:
 * Centralizar cálculos financieros relacionados con el carrito de compras,
 * asegurando consistencia y exactitud en los valores antes de persistir o mostrar información.
 *
 * Este servicio actúa como capa de lógica de dominio independiente de la base de datos o la interfaz de usuario,
 * facilitando pruebas unitarias y manteniendo el código limpio y desacoplado.
 */
namespace ApiRopa.Services.Dominio
{
    public class CarritoServicioDominio
    {
        // Calcula el subtotal de cada ítem multiplicando cantidad por precio unitario
        public void CalcularSubtotales(List<CarritoCompra> items)
        {
            foreach (var item in items)
                item.SubTotal = item.Cantidad * item.PrecioUnitario;
        }
        // Calcula el total general sumando los subtotales de todos los ítems
        public decimal CalcularTotal(List<CarritoCompra> items)
            => items.Sum(i => i.SubTotal);
    }
}

