using ApiRopa.Repositorio;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model;

namespace ApiRopa;

public class CuponRepositorio : Repositorio<Cupon>, ICuponRepositorio
{
    private readonly AppDbContext _db;

    public CuponRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    public async Task<Cupon> ActualizarCupon(Cupon entidad)
    {
        _db.Cupones.Update(entidad);
        await _db.SaveChangesAsync();
        return entidad;
    }
}
