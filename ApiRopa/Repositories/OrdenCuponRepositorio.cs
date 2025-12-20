using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Entities;

namespace ApiRopa;

public class OrdenCuponRepositorio : Repositorio<OrdenCupon>, IOrdenCuponRepositorio
{
    private readonly AppDbContext _db;

    public OrdenCuponRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    public async Task<OrdenCupon> ActualizarOrdenCupon(OrdenCupon entidad)
    {
        _db.OrdenCupones.Update(entidad);
        await _db.SaveChangesAsync();
        return entidad;
    }
}
