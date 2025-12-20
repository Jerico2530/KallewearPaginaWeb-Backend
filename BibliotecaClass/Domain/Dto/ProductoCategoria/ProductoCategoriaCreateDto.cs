using BiblotecaWeb.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiblotecaWeb.Domain.Dto.ProductoCategoria;

public class ProductoCategoriaCreateDto
{
    public int ProductoId { get; set; }
    public int CategoriaId { get; set; }
    public bool Estado { get; set; }

}
