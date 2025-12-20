using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;

namespace ApiRopa;

public class DescuentoRepositorio : Repositorio<Descuento>, IDescuentoRepositorio
{
    private readonly AppDbContext _db;

    public DescuentoRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    public async Task<Descuento> ActualizarDescuento(Descuento entidad)
    {
        _db.Descuentos.Update(entidad);
        await _db.SaveChangesAsync();
        return entidad;
    }
}
