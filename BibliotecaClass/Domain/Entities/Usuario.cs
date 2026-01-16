using BiblotecaClass.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiblotecaWeb.Domain.Entities
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }
        public string? NombreCompleto { get; set; }
        public string? ApellidoCompleto { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }
        public string DNI {  get; set; }
        public string? Imagen {  get; set; }
        public string? CorreoElectronico { get; set; }
        public string? Contraseña { get; set; }
        public string?   ContraseñaVisible { get; set; }
        public bool Estado {  get; set; }
        public DateTime FechaRegistro { get; set; }= DateTime.Now;
        public ICollection<UserRol> UserRoles { get; set; } = new List<UserRol>();
        public ICollection<ProductoFavorito> ProductoFavoritos { get; set; } = new List<ProductoFavorito>();
        public ICollection<Testimonio> Testimonios { get; set; } = new List<Testimonio>();
        public ICollection<CarritoCompra> CarritoCompras { get; set; } = new List<CarritoCompra>();
        public ICollection<Orden> Ordenes { get; set; } = new List<Orden>();
        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
        public ICollection<InfoTarjetas> InfomaTarjetas { get; set; } = new List<InfoTarjetas>();

    }
}
