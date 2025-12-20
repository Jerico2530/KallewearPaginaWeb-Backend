using ApiRopa;
using ApiRopa.Mapping;
using ApiRopa.Repositorio;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Security.Attributes;
using ApiRopa.Security.Auth;
using ApiRopa.Services;
using ApiRopa.Services.Dominio;
using ApiRopa.Services.IServices;
using ApiRopa.Servicios;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
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
using BiblotecaWeb.Domain.Dto.OrdenDetalle;
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
using BiblotecaWeb.Domain.Validacion.Anuncio;
using BiblotecaWeb.Domain.Validacion.CarritoCompra;
using BiblotecaWeb.Domain.Validacion.Categoria;
using BiblotecaWeb.Domain.Validacion.Descuento;
using BiblotecaWeb.Domain.Validacion.DetalleTarjeta;
using BiblotecaWeb.Domain.Validacion.Noticia;
using BiblotecaWeb.Domain.Validacion.OrdenDetalle;
using BiblotecaWeb.Domain.Validacion.Producto;
using BiblotecaWeb.Domain.Validacion.ProductoTalla;
using BiblotecaWeb.Domain.Validacion.Talla;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using BiblotecaWeb.Model.Validacion.Anuncio;
using BiblotecaWeb.Model.Validacion.CarritoCompra;
using BiblotecaWeb.Model.Validacion.Descuento;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

using System.Text;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Debug)
);

// 🧩 Integración con FluentValidation
builder.Services.AddFluentValidationAutoValidation();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<ExcelGenericoService>();


builder.Services.AddScoped<IValidator<CarritoCompraCreateDto>, CarritoCompraCreateValidacion>();
builder.Services.AddScoped<IValidator<CarritoCompraUpdateDto>, CarritoCompraUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, CarritoCompraGetValidacion>();
builder.Services.AddScoped<IValidator<int>, CarritoCompraDeleteValidacion>();

builder.Services.AddScoped<IValidator<AnuncioCreateDto>, AnuncioCreateValidacion>();
builder.Services.AddScoped<IValidator<AnuncioUpdateDto>, AnuncioUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, AnuncioGetValidacion>();
builder.Services.AddScoped<IValidator<int>, AnuncioDeleteValidacion>();

builder.Services.AddScoped<IValidator<CategoriaCreateDto>, CategoriaCreateValidacion>();
builder.Services.AddScoped<IValidator<CategoriaUpdateDto>, CategoriaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, CategoriaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, CategoriaDeleteValidacion>();

builder.Services.AddScoped<IValidator<DetalleTarjetaCreateDto>, DetalleTarjetaCreateValidacion>();
builder.Services.AddScoped<IValidator<DetalleTarjetaUpdateDto>, DetalleTarjetaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, DetalleTarjetaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, DetalleTarjetaDeleteValidacion>();

builder.Services.AddScoped<IValidator<DireccionCreateDto>, DireccionCreateValidacion>();
builder.Services.AddScoped<IValidator<DireccionUpdateDto>, DireccionUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, DireccionGetValidacion>();
builder.Services.AddScoped<IValidator<int>, DireccionDeleteValidacion>();

builder.Services.AddScoped<IValidator<GeneroCreateDto>, GeneroCreateValidacion>();
builder.Services.AddScoped<IValidator<GeneroUpdateDto>, GeneroUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, GeneroGetValidacion>();
builder.Services.AddScoped<IValidator<int>, GeneroDeleteValidacion > ();

builder.Services.AddScoped<IValidator<HistoriaCreateDto>, HistoriaCreateValidacion>();
builder.Services.AddScoped<IValidator<HistoriaUpdateDto>, HistoriaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, HistoriaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, HistoriaDeleteValidacion>();

builder.Services.AddScoped<IValidator<MedioPagoCreateDto>, MedioPagoCreateValidacion > ();
builder.Services.AddScoped<IValidator<MedioPagoUpdateDto>, MedioPagoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, MedioPagoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, MedioPagoDeleteValidacion>();

builder.Services.AddScoped<IValidator<MonedaCreateDto>, MoneraCreateValidacion>();
builder.Services.AddScoped<IValidator<MonedaUpdateDto>, MoneraUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, MoneraGetValidacion>();
builder.Services.AddScoped<IValidator<int>, MoneraDeleteValidacion>();

builder.Services.AddScoped<IValidator<NoticiaCreateDto>, NoticiaCreateValidacion>();
builder.Services.AddScoped<IValidator<NoticiaUpdateDto>, NoticiaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, NoticiaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, NoticiaDeleteValidacion>();

builder.Services.AddScoped<IValidator<OrdenCreateDto>, OrdenCreateValidacion>();
builder.Services.AddScoped<IValidator<OrdenUpdateDto>, OrdenUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenGetValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDeleteValidacion>();

builder.Services.AddScoped<IValidator<OrdenDetalleCreateDto>, OrdenDetalleCreateValidacion>();
builder.Services.AddScoped<IValidator<OrdenDetalleUpdateDto>, OrdenDetalleUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDetalleGetValidacion>();
builder.Services.AddScoped<IValidator<int>, OrdenDetalleDeleteValidacion>();

builder.Services.AddScoped<IValidator<PagoCreateDto>, PagoCreateValidacion>();
builder.Services.AddScoped<IValidator<PagoUpdateDto>, PagoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, PagoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, PagoDeleteValidacion>();

builder.Services.AddScoped<IValidator<PermisoCreateDto>, PermisoCreateValidacion>();
builder.Services.AddScoped<IValidator<PermisoUpdateDto>, PermisoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, PermisoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, PermisoDeleteValidacion>();

builder.Services.AddScoped<IValidator<PermRolCreateDto>, PermRolCreateValidacion>();
builder.Services.AddScoped<IValidator<PermRolUpdateDto>, PermRolUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, PermRolGetValidacion>();
builder.Services.AddScoped<IValidator<int>, PermRolDeleteValidacion>();

builder.Services.AddScoped<IValidator<PreguntaCreateDto>, PreguntaCreateValidacion>();
builder.Services.AddScoped<IValidator<PreguntaUpdateDto>, PreguntaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, PreguntaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, PreguntaDeleteValidacion>();

builder.Services.AddScoped<IValidator<ProductoCreateDto>, ProductoCreateValidacion>();
builder.Services.AddScoped<IValidator<ProductoUpdateDto>, ProductoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoDeleteValidacion>();

builder.Services.AddScoped<IValidator<ProductoCategoriaCreateDto>, ProductoCategoriaCreateValidacion>();
builder.Services.AddScoped<IValidator<ProductoCategoriaUpdateDto>, ProductoCategoriaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoCategoriaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoCategoriaDeleteValidacion>();

builder.Services.AddScoped<IValidator<ProductoTallaCreateDto>, ProductoTallaCreateValidacion>();
builder.Services.AddScoped<IValidator<ProductoTallaUpdateDto>, ProductoTallaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoTallaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, ProductoTallaDeleteValidacion>();

builder.Services.AddScoped<IValidator<RolCreateDto>, RolCreateValidacion>();
builder.Services.AddScoped<IValidator<RolUpdateDto>, RolUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, RolGetValidacion>();
builder.Services.AddScoped<IValidator<int>, RolDeleteValidacion>();

builder.Services.AddScoped<IValidator<SucursalCreateDto>, SucursalCreateValidacion>();
builder.Services.AddScoped<IValidator<SucursalUpdateDto>, SucursalUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, SucursalGetValidacion>();
builder.Services.AddScoped<IValidator<int>, SucursalDeleteValidacion>();

builder.Services.AddScoped<IValidator<TallaCreateDto>, TallaCreateValidacion>();
builder.Services.AddScoped<IValidator<TallaUpdateDto>, TallaUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, TallaGetValidacion>();
builder.Services.AddScoped<IValidator<int>, TalllaDeleteValidacion>();

builder.Services.AddScoped<IValidator<TestimonioCreateDto>, TestimonioCreateValidacion>();
builder.Services.AddScoped<IValidator<TestimonioUpdateDto>,TestimonioUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, TestimonioGetValidacion>();
builder.Services.AddScoped<IValidator<int>, TestimonioDeleteValidacion>();

builder.Services.AddScoped<IValidator<TipoPagoCreateDto>, TipoPagoCreateValidacion>();
builder.Services.AddScoped<IValidator<TipoPagoUpdateDto>, TipoPagoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, TipoPagoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, TipoPagoDeleteValidacion>();

builder.Services.AddScoped<IValidator<UserRolCreateDto>, UserRolCreateValidacion>();
builder.Services.AddScoped<IValidator<UserRolUpdateDto>, UserRolUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, UserRolGetValidacion>();
builder.Services.AddScoped<IValidator<int>, UserRolDeleteValidacion>();

builder.Services.AddScoped<IValidator<UsuarioCreateDto>, UsuarioCreateValidacion>();
builder.Services.AddScoped<IValidator<UsuarioUpdateDto>, UsuarioUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, UsuarioGetValidacion>();
builder.Services.AddScoped<IValidator<int>, UsuarioDeleteValidacion>();

builder.Services.AddScoped<IValidator<DescuentoCreateDto>, DescuentoCreateValidacion>();
builder.Services.AddScoped<IValidator<DescuentoUpdateDto>, DescuentoUpdateValidacion>();
builder.Services.AddScoped<IValidator<int>, DescuentoGetValidacion>();
builder.Services.AddScoped<IValidator<int>, DescuentoDeleteValidacion>();

builder.Services.AddScoped<IValidator<UsuarioLoginDto>, LoginCreateValidacion>();


builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<PasswordHasher>();



// Repositorios
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IProductoRepositorio, ProductoRepositorio>();
builder.Services.AddScoped<IRolRepositorio, RolRepositorio>();
builder.Services.AddScoped<IUserRolRepositorio, UserRolRepositorio>();
builder.Services.AddScoped<IAnuncioRepositorio, AnuncioRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IProductoCategoriaRepositorio, ProductoCategoriaRepositorio>();
builder.Services.AddScoped<IProductoFavoritoRepositorio, ProductoFavoritoRepositorio>();
builder.Services.AddScoped<ITestimonioRepositorio, TestimonioRepositorio>();
builder.Services.AddScoped<ICarritoCompraRepositorio, CarritoCompraRepositorio>();
builder.Services.AddScoped<IGeneroRepositorio, GeneroRepositorio>();
builder.Services.AddScoped<IHistoriaRepositorio, HistoriaRepositorio>();
builder.Services.AddScoped<IMonedaRepositorio, MonedaRepositorio>();
builder.Services.AddScoped<INoticiaRepositorio, NoticiaRepositorio>();
builder.Services.AddScoped<IProductoTallaRepositorio, ProductoTallaRepositorio>();
builder.Services.AddScoped<ITallaRepositorio, TallaRepositorio>();
builder.Services.AddScoped<IDescuentoRepositorio, DescuentoRepositorio>();
builder.Services.AddScoped<IPermisoRepositorio, PermisoRepositorio>();
builder.Services.AddScoped<IPermRolRepositorio, PermRolRepositorio>();
builder.Services.AddScoped<IPreguntaRepositorio, PreguntaRepositorio>();
builder.Services.AddScoped<IOrdenRepositorio, OrdenRepositorio>();
builder.Services.AddScoped<IOrdenCuponRepositorio, OrdenCuponRepositorio>();
builder.Services.AddScoped<ICuponRepositorio, CuponRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>();
builder.Services.AddScoped<IOrdenDetalleRepositorio, OrdenDetalleRepositorio>();
builder.Services.AddScoped<ISucursalRepositorio, SucursalRepositorio>();
builder.Services.AddScoped<ITipoPagoRepositorio, TipoPagoRepositorio>();
builder.Services.AddScoped<IMedioPagoRepositorio, MedioPagoRepositorio>();
builder.Services.AddScoped<IDetalleTarjetaRepositorio, DetalleTarjetaRepositorio>();
builder.Services.AddScoped<IDireccionRepositorio, DireccionRepositorio>();



// Repositorio genérico (⚡ necesario para IRepositorio<Producto>, IRepositorio<Talla>, etc.)
builder.Services.AddScoped(typeof(IRepositorio<>), typeof(Repositorio<>));

//Services
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IUserRolService, UserRolService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IAnuncioService, AnuncioService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>(); 
builder.Services.AddScoped<IProductoCategoriaService, ProductoCategoriaService>();
builder.Services.AddScoped<ITestimonioService, TestimonioService>();
builder.Services.AddScoped<ICarritoCompraService, CarritoCompraService>();
builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IHistoriaService, HistoriaService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<INoticiaService, NoticiaService>();
builder.Services.AddScoped<IProductoTallaService, ProductoTallaService>();
builder.Services.AddScoped<ITallaService, TallaService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IPermRolService, PermRolService>();
builder.Services.AddScoped<IPreguntaService, PreguntaService>();
builder.Services.AddScoped<IOrdenDetalleService, OrdenDetalleService>();
builder.Services.AddScoped<IOrdenService, OrdenService>();
builder.Services.AddScoped<IOrdenDetalleService, OrdenDetalleService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<ITipoPagoService, TipoPagoService>();
builder.Services.AddScoped<IMedioPagoService, MedioPagoService>();
builder.Services.AddScoped<IDetalleTarjetaService, DetalleTarjetaService>();
builder.Services.AddScoped<IDireccionService, DireccionService>();
builder.Services.AddScoped<IDescuentoService, DescuentoService>();
builder.Services.AddScoped<CarritoServicioDominio>();
builder.Services.AddScoped<ILoginService, LoginService>();





// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NewPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();
