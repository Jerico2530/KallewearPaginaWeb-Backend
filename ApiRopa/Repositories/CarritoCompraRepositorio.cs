using ApiRopa.Repositorio;
using ApiRopa.Repositorio.IRepositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
/*
     * CarritoCompraRepositorio
     *
     * Repositorio especializado para la entidad CarritoCompra.
     * Funcionalidades clave:
     * - Gestiona el CRUD de CarritoCompra mediante EF Core.
     * - Permite obtener carritos con sus relaciones completas (Usuario, Producto, Talla, Orden, Moneda, Género).
     * - Permite calcular totales, vaciar carritos por usuario y gestionar carritos de invitados.
     *
     * Propósito del componente:
     * Proporcionar un acceso consistente y optimizado a los datos del carrito de compras,
     * facilitando la integración con la lógica de negocio del comercio electrónico.
     *
     * Descripción del código:
     * - Constructor: inicializa el contexto de base de datos y la clase base genérica.
     * - Métodos CRUD específicos: ActualizarCarritoCompra, ObtenerCarritoCompraConDetalles, etc.
     * - Métodos auxiliares: CalcularTotalAsync, VaciarCarritoPorUsuarioAsync, ObtenerIdInvitadoActualAsync.
     */
namespace ApiRopa
{
    public class CarritoCompraRepositorio : Repositorio<CarritoCompra>, ICarritoCompraRepositorio
    {
        private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

        public CarritoCompraRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza un carrito de compras existente en la base de datos
        public async Task<CarritoCompra> ActualizarCarritoCompra(CarritoCompra entidad)
        {
            _db.CarritoCompras.Update(entidad); // Marca la entidad como modificada
            await _db.SaveChangesAsync();      // Persiste los cambios en la base de datos
            return entidad;                     // Retorna la entidad actualizada
        }
        /// Obtiene todos los carritos con sus detalles relacionados (Usuario, Orden, ProductoTalla y subrelaciones)
        public async Task<List<CarritoCompra>> ObtenerCarritoCompraConDetalles()
        {
            return await _db.CarritoCompras
                .AsNoTracking()
                .Include(c => c.Usuario)
                .Include(c => c.Orden)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Producto)
                        .ThenInclude(p => p.Moneda)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Producto)
                        .ThenInclude(p => p.Genero)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Talla)
                .ToListAsync();
        }

        /// Obtiene un carrito por su ID con sus detalles relacionados
        public async Task<CarritoCompra?> ObtenerCarritoCompraConDetallesPorId(int id)
        {
            return await _db.CarritoCompras
                .AsNoTracking()
                .Include(ur => ur.Usuario)
                .Include(ur => ur.Orden)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Producto)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Talla)
                .FirstOrDefaultAsync(p => p.CarritoId == id);
        }

        /// Obtiene todos los carritos de un usuario que aún no están asociados a una orden
        public async Task<List<CarritoCompra>> ObtenerCarritoPorUsuarioConDetalles(int usuarioId)
        {
            return await _db.CarritoCompras
                .AsNoTracking()
                .Where(c => c.UsuarioId == usuarioId) // 🔥 FILTRO CLAVE
                .Include(c => c.Usuario)
                .Include(c => c.Orden)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Producto)
                        .ThenInclude(p => p.Moneda)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Producto)
                        .ThenInclude(p => p.Genero)
                .Include(c => c.ProductoTalla)
                    .ThenInclude(pt => pt.Talla)
                .ToListAsync();
        }

        /// Elimina todos los carritos de un usuario
        public async Task VaciarCarritoPorUsuarioAsync(int usuarioId)
        {
            var carrito = await _db.CarritoCompras
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            if (carrito != null && carrito.Any())
            {
                _db.CarritoCompras.RemoveRange(carrito);
                await _db.SaveChangesAsync();
            }
        }

        /// Calcula el total del carrito de un usuario y actualiza cada ítem con el total general
        public async Task<decimal> CalcularTotalAsync(int usuarioId)
        {
            var carrito = await _db.CarritoCompras
                .Where(c => c.UsuarioId == usuarioId)
                .ToListAsync();

            if (!carrito.Any())
            {
                return 0; // si ya no quedan productos, total = 0
            }

            decimal totalGeneral = carrito.Sum(item => item.SubTotal);

            foreach (var item in carrito)
            {
                item.TotalCarrito = totalGeneral;
            }

            _db.CarritoCompras.UpdateRange(carrito);
            await _db.SaveChangesAsync();

            return totalGeneral;
        }

        /// Obtiene todos los carritos que cumplen un filtro específico
        public async Task<IEnumerable<CarritoCompra>> ObtenerTodos(Expression<Func<CarritoCompra, bool>> filtro)
        {
            return await _db.CarritoCompras
                .Where(filtro)
                .ToListAsync();
        }
        /// Actualiza una entidad de carrito específica
        public async Task Actualizar(CarritoCompra entidad)
        {
            _db.CarritoCompras.Update(entidad);
            await Grabar();
        }
        /// Obtiene el ID del usuario invitado actual
        public async Task<int> ObtenerIdInvitadoActualAsync()
        {
            var invitado = await _db.Usuarios
                .Where(u => u.UserRoles.Any(ur => ur.Rol.NombreRol == "Invitado"))
                .Select(u => u.UsuarioId)
                .FirstOrDefaultAsync();

            return invitado;
        }
        /// Obtiene un carrito según un filtro, opcionalmente sin tracking para EF Core
        public async Task<CarritoCompra> Obtener(Expression<Func<CarritoCompra, bool>> filtro, bool tracked = false)
        {
            IQueryable<CarritoCompra> query = _db.CarritoCompras;

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(filtro);
        }

        public async Task<CarritoCompra> CrearAsync(CarritoCompra carrito)
        {
            _db.CarritoCompras.Add(carrito);
            await _db.SaveChangesAsync();
            return carrito;
        }

        public async Task EliminarItemAsync(int carritoId)
        {
            var item = await _db.CarritoCompras
                .FirstOrDefaultAsync(c => c.CarritoId == carritoId);

            if (item != null)
            {
                _db.CarritoCompras.Remove(item);
                await _db.SaveChangesAsync();
            }
        }
    }
}
