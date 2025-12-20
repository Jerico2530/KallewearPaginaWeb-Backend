using BiblotecaWeb;
using BiblotecaWeb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Proyecto Empresarial – Capa de Persistencia (Entity Framework Core)
 * ------------------------------------------------------------------
 * Este componente define el contexto de base de datos principal del sistema.
 * Centraliza la configuración de entidades, relaciones y reglas de eliminación
 * para garantizar integridad referencial, escalabilidad y consistencia de datos.
 *
 * Funcionalidades clave:
 * - Exponer los DbSet que representan el modelo de dominio.
 * - Configurar claves primarias y relaciones entre entidades.
 * - Definir comportamientos de eliminación alineados a reglas de negocio.
 *
 * Propósito del componente:
 * Actuar como punto único de acceso y configuración de la base de datos,
 * desacoplando la lógica de persistencia del resto de la aplicación.
 *
 * Descripción general del código:
 * - Se definen las entidades principales del dominio.
 * - Se configuran relaciones uno a muchos y tablas intermedias.
 * - Se prioriza DeleteBehavior.Restrict para proteger información histórica
 *   y DeleteBehavior.Cascade solo en entidades dependientes.
 */

namespace BiblotecaWeb.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<CarritoCompra> CarritoCompras { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cupon> Cupones { get; set; }
        public DbSet<Descuento> Descuentos { get; set; }
        public DbSet<DetalleTarjeta> DetalleTarjetas { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Historia> Historias { get; set; }
        public DbSet<MedioPago> MedioPagos { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Noticia> Noticias { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<OrdenCupon> OrdenCupones { get; set; }
        public DbSet<OrdenDetalle> OrdenDetalles { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<PermRol> PermRoles { get; set; }
        public DbSet<Pregunta> Preguntas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoCategoria> ProductoCategorias { get; set; }
        public DbSet<ProductoFavorito> ProductoFavoritos { get; set; }
        public DbSet<ProductoTalla> ProductoTallas { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Talla> Tallas { get; set; }
        public DbSet<Testimonio> Testimonios { get; set; }
        public DbSet<TipoPago> TipoPagos { get; set; }
        public DbSet<UserRol> UserRoles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // Configuración centralizada de relaciones, claves y reglas del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo explícito de tablas (evita dependencias de convenciones)
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<UserRol>().ToTable("UserRol");
            modelBuilder.Entity<Anuncio>().ToTable("Anuncio");
            modelBuilder.Entity<CarritoCompra>().ToTable("CarritoCompra");
            modelBuilder.Entity<Categoria>().ToTable("Categoria");
            modelBuilder.Entity<Cupon>().ToTable("Cupon");
            modelBuilder.Entity<Descuento>().ToTable("Descuento");
            modelBuilder.Entity<DetalleTarjeta>().ToTable("DetalleTarjeta");
            modelBuilder.Entity<Direccion>().ToTable("Direccion");
            modelBuilder.Entity<Genero>().ToTable("Genero");
            modelBuilder.Entity<Historia>().ToTable("Historia");
            modelBuilder.Entity<MedioPago>().ToTable("MedioPago");
            modelBuilder.Entity<Moneda>().ToTable("Moneda");
            modelBuilder.Entity<Noticia>().ToTable("Noticia");
            modelBuilder.Entity<Orden>().ToTable("Orden");
            modelBuilder.Entity<OrdenCupon>().ToTable("OrdenCupon");
            modelBuilder.Entity<OrdenDetalle>().ToTable("OrdenDetalle");
            modelBuilder.Entity<Pago>().ToTable("Pago");
            modelBuilder.Entity<Permiso>().ToTable("Permiso");
            modelBuilder.Entity<PermRol>().ToTable("PermRol");
            modelBuilder.Entity<Pregunta>().ToTable("Pregunta");
            modelBuilder.Entity<Producto>().ToTable("Producto");
            modelBuilder.Entity<ProductoCategoria>().ToTable("ProductoCategoria");
            modelBuilder.Entity<ProductoFavorito>().ToTable("ProductoFavorito");
            modelBuilder.Entity<ProductoTalla>().ToTable("ProductoTalla");
            modelBuilder.Entity<Rol>().ToTable("Rol");
            modelBuilder.Entity<Sucursal>().ToTable("Sucursal");
            modelBuilder.Entity<Talla>().ToTable("Talla");
            modelBuilder.Entity<Testimonio>().ToTable("Testimonio");
            modelBuilder.Entity<TipoPago>().ToTable("TipoPago");


            // Usuario - Rol (N:M)
            modelBuilder.Entity<UserRol>()
                 .HasKey(ur => ur.UserRolId);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RolId)
                .OnDelete(DeleteBehavior.Cascade);


            // Pago - DetalleTarjeta (1:N)
            modelBuilder.Entity<DetalleTarjeta>()
                .HasKey(ur => ur.DetalleTarjetaId);

            modelBuilder.Entity<DetalleTarjeta>()
                .HasOne(ur => ur.Pago)
                .WithMany(u => u.DetalleTarjetas)
                .HasForeignKey(ur => ur.PagoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Dirección (1:N)
            modelBuilder.Entity<Direccion>()
                .HasKey(ur => ur.DireccionId);

            modelBuilder.Entity<Direccion>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.Direcciones)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // TipoPago - MedioPago (1:N)
            modelBuilder.Entity<MedioPago>()
                .HasKey(ur => ur.MedioPagoId);

            modelBuilder.Entity<MedioPago>()
                .HasOne(ur => ur.TipoPago)
                .WithMany(u => u.MediosPagos)
                .HasForeignKey(ur => ur.TipoPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Orden (1:N)
            modelBuilder.Entity<Orden>()
                .HasKey(ur => ur.OrdenId);

            modelBuilder.Entity<Orden>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.Ordenes)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sucursal - Orden (1:N)
            modelBuilder.Entity<Orden>()
                .HasOne(ur => ur.Sucursal)
                .WithMany(u => u.Ordenes)
                .HasForeignKey(ur => ur.SucursalId)
                .OnDelete(DeleteBehavior.Restrict);

            // Dirección - Orden (1:N)
            modelBuilder.Entity<Orden>()
                .HasOne(ur => ur.Direccion)
                .WithMany(u => u.Ordenes)
                .HasForeignKey(ur => ur.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Orden - Detalle (1:N)
            modelBuilder.Entity<OrdenDetalle>()
                .HasKey(ur => ur.OrdenDetalleId);

            modelBuilder.Entity<OrdenDetalle>()
                .HasOne(ur => ur.Orden)
                .WithMany(u => u.OrdenDetalles)
                .HasForeignKey(ur => ur.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto - DetalleOrden (1:N)
            modelBuilder.Entity<OrdenDetalle>()
                .HasOne(ur => ur.Producto)
                .WithMany(u => u.OrdenDetalles)
                .HasForeignKey(ur => ur.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Orden - Pago (1:N)
            modelBuilder.Entity<Pago>()
                .HasKey(ur => ur.PagoId);

            modelBuilder.Entity<Pago>()
                .HasOne(ur => ur.Orden)
                .WithMany(u => u.Pagos)
                .HasForeignKey(ur => ur.OrdenId)
               .OnDelete(DeleteBehavior.Restrict);

            // MedioPago - Pago (1:N)
            modelBuilder.Entity<Pago>()
                .HasOne(ur => ur.MedioPago)
                .WithMany(u => u.Pagos)
                .HasForeignKey(ur => ur.MedioPagoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rol - Permiso(N: M)
            modelBuilder.Entity<PermRol>()
                .HasKey(ur => ur.PermRolId);

            modelBuilder.Entity<PermRol>()
                .HasOne(ur => ur.Permiso)
                .WithMany(u => u.PermRoles)
                .HasForeignKey(ur => ur.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PermRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(u => u.PermRoles)
                .HasForeignKey(ur => ur.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            //Moneda  - Producto (1:N)
            modelBuilder.Entity<Producto>()
                .HasKey(ur => ur.ProductoId);

            modelBuilder.Entity<Producto>()
                .HasOne(ur => ur.Moneda)
                .WithMany(u => u.Productos)
                .HasForeignKey(ur => ur.MonedaId)
                .OnDelete(DeleteBehavior.Restrict);

            //Genero  - Producto (1:N)
            modelBuilder.Entity<Producto>()
                .HasOne(ur => ur.Genero)
                .WithMany(u => u.Productos)
                .HasForeignKey(ur => ur.GeneroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Producto - Categoria(N: M)
            modelBuilder.Entity<ProductoCategoria>()
                .HasKey(ur => ur.ProductoCategoriaId);

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(ur => ur.Producto)
                .WithMany(u => u.ProductoCategorias)
                .HasForeignKey(ur => ur.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(ur => ur.Categoria)
                .WithMany(u => u.ProductoCategorias)
                .HasForeignKey(ur => ur.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);

            //Producto  - ProductoFavorito (1:N)
            modelBuilder.Entity<ProductoFavorito>()
               .HasKey(ur => ur.ProductoFavoritoId);

            modelBuilder.Entity<ProductoFavorito>()
                .HasOne(ur => ur.Producto)
                .WithMany(u => u.ProductoFavoritos)
                .HasForeignKey(ur => ur.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            //Usuario  - ProductoFavorito (1:N)
            modelBuilder.Entity<ProductoFavorito>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.ProductoFavoritos)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Talla - Producto(N: M)
            modelBuilder.Entity<ProductoTalla>()
               .HasKey(ur => ur.ProductoTallaId);

            modelBuilder.Entity<ProductoTalla>()
                .HasOne(ur => ur.Producto)
                .WithMany(u => u.ProductoTallas)
                .HasForeignKey(ur => ur.ProductoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductoTalla>()
                .HasOne(ur => ur.Talla)
                .WithMany(u => u.ProductoTallas)
                .HasForeignKey(ur => ur.TallaId)
                .OnDelete(DeleteBehavior.Cascade);

            // ProductoTalla - Carrito (1:N)
            modelBuilder.Entity<CarritoCompra>()
               .HasKey(ur => ur.CarritoId);

            modelBuilder.Entity<CarritoCompra>()
                .HasOne(ur => ur.ProductoTalla)
                .WithMany(u => u.CarritoCompras)
                .HasForeignKey(ur => ur.ProductoTallaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario - Carrito (1:N)
            modelBuilder.Entity<CarritoCompra>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.CarritoCompras)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Orden - Carrito (1:N)
            modelBuilder.Entity<CarritoCompra>()
                .HasOne(ur => ur.Orden)
                .WithMany(u => u.CarritoCompras)
                .HasForeignKey(ur => ur.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
