using ApiRopa.Repositories.Interfaces;
using ApiRopa.Repositorio;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiRopa.Repositories
{
    public class InfoTarjetaRepositorio : Repositorio<InfoTarjetas>, IInfoTarjetaRepositorio
    {
        private readonly AppDbContext _db; // Contexto EF Core para acceso a la base de datos

        public InfoTarjetaRepositorio(AppDbContext db) : base(db)
        {
            _db = db;
        }
        /// Actualiza una historia existente en la base de datos
        public async Task<InfoTarjetas> ActualizarInfoTarjeta(InfoTarjetas entidad)
        {
            _db.InfomaTarjetas.Update(entidad); // Marca la entidad como modificada
            await _db.SaveChangesAsync(); // Persiste los cambios
            return entidad;               // Retorna la entidad actualizada
        }

        public async Task<List<InfoTarjetas>> ObtenerInfoTarjetasConDetalles()
        {
            return await _db.InfomaTarjetas
                 .Include(t => t.Usuario)          // Incluye información del usuario
                 .Include(t => t.DetalleTarjeta)   // Incluye detalles de la tarjeta
                 .Include(t => t.MedioPago)        // Incluye el medio de pago
                     .ThenInclude(mp => mp.TipoPago) // Incluye el tipo de pago
                 .ToListAsync();          // Devuelve la lista completa
        }

        public async Task<List<InfoTarjetas>> ObtenerInfoTarjetasPorUsuarioAsync(int usuarioId)
        {
            return await _db.InfomaTarjetas
                .Include(t => t.Usuario)
                .Include(t => t.DetalleTarjeta)
                .Include(t => t.MedioPago)
                    .ThenInclude(mp => mp.TipoPago)
                .Where(t => t.UsuarioId == usuarioId)
                .ToListAsync();
        }


    }
}
