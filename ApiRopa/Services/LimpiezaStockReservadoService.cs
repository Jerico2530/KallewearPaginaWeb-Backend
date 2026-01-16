using BiblotecaWeb.Datos;
using Microsoft.EntityFrameworkCore;

namespace ApiRopa.Services
{
    public class LimpiezaStockReservadoService
    {
        private readonly AppDbContext _db;

            public LimpiezaStockReservadoService(AppDbContext db)
            {
                _db = db;
            }

            /// <summary>
            /// Libera stock reservado de carritos abandonados
            /// </summary>
            public async Task LimpiarReservasAsync(int minutosExpiracion = 30)
            {
                var fechaLimite = DateTime.Now.AddMinutes(-minutosExpiracion);

                // 1️⃣ Buscar carritos viejos que aún reservan stock y NO están pagados
                var carritosAbandonados = await _db.CarritoCompras
                    .Include(c => c.ProductoTalla)
                    .Where(c =>
                        c.Estado == true &&       // sigue activo
                        c.OrdenId == null &&      // no está asociado a pago confirmado
                        c.FechaRegistro <= fechaLimite
                    )
                    .ToListAsync();

                if (!carritosAbandonados.Any())
                    return;

                // 2️⃣ Liberar stock reservado
                foreach (var carrito in carritosAbandonados)
                {
                    carrito.ProductoTalla.StockReservado -= carrito.Cantidad;
                    if (carrito.ProductoTalla.StockReservado < 0)
                        carrito.ProductoTalla.StockReservado = 0;

                    carrito.Estado = false; // marcar como abandonado

                    _db.ProductoTallas.Update(carrito.ProductoTalla);
                    _db.CarritoCompras.Update(carrito);
                }

                await _db.SaveChangesAsync();
            }
        }
    }

 
