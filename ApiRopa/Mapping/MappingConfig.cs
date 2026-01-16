using AutoMapper;
using BiblotecaClass.Domain.Dto.InfoTarjetas;
using BiblotecaClass.Domain.Entities;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.Anuncio;
using BiblotecaWeb.Domain.Dto.Categoria;
using BiblotecaWeb.Domain.Dto.Descuento;
using BiblotecaWeb.Domain.Dto.DetalleTarjeta;
using BiblotecaWeb.Domain.Dto.Direccion;
using BiblotecaWeb.Domain.Dto.Genero;
using BiblotecaWeb.Domain.Dto.Historia;
using BiblotecaWeb.Domain.Dto.MedioPago;
using BiblotecaWeb.Domain.Dto.Moneda;
using BiblotecaWeb.Domain.Dto.Noticia;
using BiblotecaWeb.Domain.Dto.Orden;
using BiblotecaWeb.Domain.Dto.Pago;
using BiblotecaWeb.Domain.Dto.Permiso;
using BiblotecaWeb.Domain.Dto.PermRol;
using BiblotecaWeb.Domain.Dto.Pregunta;
using BiblotecaWeb.Domain.Dto.Producto;
using BiblotecaWeb.Domain.Dto.ProductoCategoria;
using BiblotecaWeb.Domain.Dto.ProductoTalla;
using BiblotecaWeb.Domain.Dto.Rol;
using BiblotecaWeb.Domain.Dto.Sucursal;
using BiblotecaWeb.Domain.Dto.Talla;
using BiblotecaWeb.Domain.Dto.Testimonio;
using BiblotecaWeb.Domain.Dto.TipoPago;
using BiblotecaWeb.Domain.Dto.UserRol;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using BiblotecaWeb.Model.Dto;

namespace ApiRopa.Mapping
{
    /// <summary>
    /// Configuración central de AutoMapper para el proyecto.
    /// Contiene todos los mapeos entre Entidades y DTOs.
    /// </summary>
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            //PRODUCTO
            CreateMap<Producto, ProductoCreateDto>().ReverseMap();
            CreateMap<Producto, ProductoDto>()
    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Genero.Tipo ))
    .ForMember(dest => dest.Codigo, opt => opt.MapFrom(src => src.Moneda.Codigo))
            .ForMember(dest => dest.MonedaId, opt => opt.MapFrom(src => src.MonedaId))
    .ForMember(dest => dest.GeneroId, opt => opt.MapFrom(src => src.GeneroId));
            CreateMap<Producto, ProductoUpdateDto>().ReverseMap();

            //USUARIO
            CreateMap<Usuario, UsuarioCreateDto>() .ReverseMap();
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioUpdateDto>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginDto>().ReverseMap();

            //ROL
            CreateMap<Rol, RolCreateDto>().ReverseMap();
            CreateMap<Rol, RolDto>().ReverseMap();
            CreateMap<Rol, RolUpdateDto>().ReverseMap();

            //USERROL
            CreateMap<UserRol,UserRolCreateDto>().ReverseMap();
            CreateMap<UserRol, UserRolDto>()
    .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Usuario.NombreCompleto ))
    .ForMember(dest => dest.ApellidoCompleto, opt => opt.MapFrom(src => src.Usuario.ApellidoCompleto))
    .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.Usuario.DNI))
    .ForMember(dest => dest.CorreoElectronico, opt => opt.MapFrom(src => src.Usuario.CorreoElectronico))
    .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol.NombreRol));
            CreateMap<UserRol, UserRolUpdateDto>().ReverseMap();

            //ANUNCIO
            CreateMap<Anuncio, AnuncioCreateDto>().ReverseMap();
            CreateMap<Anuncio, AnuncioDto>().ReverseMap();
            CreateMap<Anuncio, AnuncioUpdateDto>().ReverseMap();

            //PRODUCTO CATEGORIA
            CreateMap<ProductoCategoria, ProductoCategoriaCreateDto>().ReverseMap();
            CreateMap<ProductoCategoria, ProductoCategoriaDto>()
    .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Producto.Nombre))
    .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Producto.Descripcion))
    .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Producto.Precio))
    .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Producto.Imagen))
    .ForMember(dest => dest.DesCategoria, opt => opt.MapFrom(src => src.Categoria.DesCategoria));
            CreateMap<ProductoCategoriaUpdateDto, ProductoCategoria>().ReverseMap();

            //CATEGORIA
            CreateMap<Categoria, CategoriaCreateDto>().ReverseMap();
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CategoriaUpdateDto>().ReverseMap();

            //TESTIMONIO
            CreateMap<Testimonio, TestimonioCreateDto>().ReverseMap();
            CreateMap<Testimonio, TestimonioDto>()
     .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Usuario.NombreCompleto))
     .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Usuario.Imagen));
            CreateMap<Testimonio, TestimonioUpdateDto>().ReverseMap();

            //TALLA
            CreateMap<Talla, TallaCreateDto>().ReverseMap();
            CreateMap<Talla, TallaDto>().ReverseMap();
            CreateMap<Talla, TallaUpdateDto>().ReverseMap();

            //CARRITO COMPRA
            CreateMap<CarritoCompra, CarritoCompraCreateDto>().ReverseMap();
            CreateMap<CarritoCompra, CarritoCompraDto>()
    .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Usuario.NombreCompleto))
    .ForMember(dest => dest.ApellidoCompleto, opt => opt.MapFrom(src => src.Usuario.ApellidoCompleto))
    .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.Usuario.DNI))
    .ForMember(dest => dest.CorreoElectronico, opt => opt.MapFrom(src => src.Usuario.CorreoElectronico))
    .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Nombre))
    .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Descripcion))
    .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Precio))
    .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Imagen))
    .ForMember(dest => dest.Moneda, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Moneda.Codigo))
    .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.ProductoTalla.Producto.Genero.Tipo))
    .ForMember(dest => dest.TipoTalla, opt => opt.MapFrom(src => src.ProductoTalla.Talla.TipoTalla))
    .ForMember(dest => dest.Orden, opt => opt.MapFrom(src => src.Orden))
             .ForMember(dest => dest.ProductoId, opt => opt.MapFrom(src => src.ProductoTalla.ProductoId))
    .ForMember(dest => dest.TallaId, opt => opt.MapFrom(src => src.ProductoTalla.TallaId));
            CreateMap<Orden, OrdenSimpleDto>();
            CreateMap<CarritoCompra, CarritoCompraUpdateDto>().ReverseMap();

            //GENERO
            CreateMap<Genero, GeneroCreateDto>().ReverseMap();
            CreateMap<Genero, GeneroDto>().ReverseMap();
            CreateMap<Genero, GeneroUpdateDto>().ReverseMap();

            //HISTORIA
            CreateMap<Historia, HistoriaCreateDto>().ReverseMap();
            CreateMap<Historia, HistoriaDto>().ReverseMap();
            CreateMap<Historia, HistoriaUpdateDto>().ReverseMap();

            //MONEDA
            CreateMap<Moneda, MonedaCreateDto>().ReverseMap();
            CreateMap<Moneda, MonedaDto>().ReverseMap();
            CreateMap<Moneda, MonedaUpdateDto>().ReverseMap();

            //NOTICIA
            CreateMap<Noticia, NoticiaCreateDto>().ReverseMap();
            CreateMap<Noticia, NoticiaDto>().ReverseMap();
            CreateMap<Noticia, NoticiaUpdateDto>().ReverseMap();

            //PRODUCTO TALLA
            CreateMap<ProductoTalla, ProductoTallaCreateDto>().ReverseMap();
            CreateMap<ProductoTalla, ProductoTallaDto>()
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Producto.Nombre))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Producto.Descripcion))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Producto.Precio))
                .ForMember(dest => dest.Moneda, opt => opt.MapFrom(src => src.Producto.Moneda.Codigo))  
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Producto.Genero.Tipo))
                .ForMember(dest => dest.Imagen, opt => opt.MapFrom(src => src.Producto.Imagen))
                .ForMember(dest => dest.Categoria,opt => opt.MapFrom(src =>src.Producto.ProductoCategorias.Select(pc => pc.Categoria.DesCategoria).FirstOrDefault()))
                .ForMember(dest => dest.TipoTalla, opt => opt.MapFrom(src => src.Talla.TipoTalla));
            CreateMap<ProductoTalla, ProductoTallaUpdateDto>().ReverseMap();

            //DESCUENTO
            CreateMap<Descuento, DescuentoCreateDto>().ReverseMap();
            CreateMap<Descuento, DescuentoDto>().ReverseMap();
            CreateMap<Descuento, DescuentoUpdateDto>().ReverseMap();

            //PERMISO
            CreateMap<Permiso, PermisoCreateDto>().ReverseMap();
            CreateMap<Permiso, PermisoDto>().ReverseMap();
            CreateMap<Permiso, PermisoUpdateDto>().ReverseMap();

            //PERMISO ROL
            CreateMap<PermRol, PermRolCreateDto>().ReverseMap();
            CreateMap<PermRol, PermRolDto>()
    .ForMember(dest => dest.NombrePermiso, opt => opt.MapFrom(src => src.Permiso.NombrePermiso))
    .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol.NombreRol));
            CreateMap<PermRol, PermRolUpdateDto>().ReverseMap();

            //PREGUNTA
            CreateMap<Pregunta, PreguntaCreateDto>().ReverseMap();
            CreateMap<Pregunta, PreguntaDto>().ReverseMap();
            CreateMap<Pregunta, PreguntaUpdateDto>().ReverseMap();

            //ORDEN
            CreateMap<Orden, OrdenCreateDto>().ReverseMap();
            CreateMap<Orden, OrdenDto>()
    .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => src.Usuario.NombreCompleto))
    .ForMember(dest => dest.ApellidoCompleto, opt => opt.MapFrom(src => src.Usuario.ApellidoCompleto))
    .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.Usuario.DNI))
    .ForMember(dest => dest.Locales, opt => opt.MapFrom(src => src.Sucursal.Locales))
    .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Sucursal.Descripcion))
    .ForMember(dest => dest.Departamente, opt => opt.MapFrom(src => src.Direccion.Departamento))
    .ForMember(dest => dest.Provincia, opt => opt.MapFrom(src => src.Direccion.Provincia))
    .ForMember(dest => dest.Distrito, opt => opt.MapFrom(src => src.Direccion.Distrito))
    .ForMember(dest => dest.Via, opt => opt.MapFrom(src => src.Direccion.Via))
    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.Direccion.Numero))
    .ForMember(dest => dest.CarritoCompras, opt => opt.MapFrom(src => src.CarritoCompras))
    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));
            CreateMap<Orden, OrdenUpdateDto>().ReverseMap();

            //SUCURSAL
            CreateMap<Sucursal, SucursalCreateDto>().ReverseMap();
            CreateMap<Sucursal, SucursalDto>().ReverseMap();
            CreateMap<Sucursal, SucursalUpdateDto>().ReverseMap();

            //TIPO PAGO
            CreateMap<TipoPago, TipoPagoCreateDto>().ReverseMap();
            CreateMap<TipoPago, TipoPagoDto>().ReverseMap();
            CreateMap<TipoPago, TipoPagoUpdateDto>().ReverseMap();

            //MEDIO PAGO
            CreateMap<MedioPago, MedioPagoCreateDto>().ReverseMap();
            CreateMap<MedioPago, MedioPagoDto>()
            .ForMember(dest => dest.DescripcionTipoPago, opt => opt.MapFrom(src => src.TipoPago.DescripcionTipoPago));
            CreateMap<MedioPago, MedioPagoUpdateDto>().ReverseMap();

            //DETALLE TARJETA
            CreateMap<DetalleTarjeta, DetalleTarjetaCreateDto>().ReverseMap();
            CreateMap<DetalleTarjeta, DetalleTarjetaDto>();
            CreateMap<DetalleTarjeta, DetalleTarjetaUpdateDto>().ReverseMap();

            //PAGO
            CreateMap<Pago, PagoCreateDto>().ReverseMap();
            CreateMap<Pago, PagoDto>()
    .ForMember(dest => dest.MetodoEntrega, opt => opt.MapFrom(src => src.Orden.MetodoEntrega))
    .ForMember(dest => dest.DescripcionMedioPago, opt => opt.MapFrom(src => src.MedioPago.DescripcionMedioPago))
    .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.MedioPago.TipoPago.DescripcionTipoPago));
            CreateMap<Pago, PagoUpdateDto>().ReverseMap();

            //DIRECCION
            CreateMap<Direccion, DireccionCreateDto>().ReverseMap();
            CreateMap<Direccion, DireccionDto>().ReverseMap();
            CreateMap<Direccion, DireccionUpdateDto>().ReverseMap();

            //InfoTarjeta
            CreateMap<InfoTarjetas, InfoTarjetaCreateDto>().ReverseMap();
            CreateMap<InfoTarjetas, InfoTarjetaDto>()
    .ForMember(dest => dest.NumeroTarjeta, opt => opt.MapFrom(src => src.DetalleTarjeta.NumeroTarjeta))
    .ForMember(dest => dest.FechaVencimiento, opt => opt.MapFrom(src => src.DetalleTarjeta.FechaVencimiento))
    .ForMember(dest => dest.CVV, opt => opt.MapFrom(src => src.DetalleTarjeta.CVV))
    .ForMember(dest => dest.DescripcionMedioPago, opt => opt.MapFrom(src => src.MedioPago.DescripcionMedioPago))
    .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.MedioPago.TipoPago.DescripcionTipoPago));
            CreateMap<InfoTarjetas, InfoTarjetaUpdateDto>().ReverseMap();

        }
    }
}
