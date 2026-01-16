using ApiRopa.Models;
using ApiRopa.Models.Dtos;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Security.Auth;
using ApiRopa.Services.Help;
using ApiRopa.Services.IServices;
using AutoMapper;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
/*
 * LoginService
 *
 * Servicio encargado de gestionar la autenticación de usuarios y la gestión de carritos asociados.
 * Funcionalidades clave:
 * - Login de usuarios registrados con validación de credenciales.
 * - Login de usuarios invitados con generación de token temporal.
 * - Migración de carritos de usuarios invitados a usuarios registrados al iniciar sesión.
 * - Generación de JWT y asignación de roles y permisos.
 * - Validaciones mediante FluentValidation.
 *
 * Propósito del componente:
 * Centralizar la lógica de autenticación y gestión de sesiones de usuarios,
 * garantizando integridad de datos, seguridad y consistencia en la migración de carritos.
 *
 * Actúa como capa intermedia entre controladores y repositorios, asegurando
 * que las operaciones se realicen correctamente y manteniendo el código
 * limpio, mantenible y desacoplado de la capa de datos.
 */
namespace ApiRopa.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<LoginService> _logger;

        private readonly JwtService _utilidades;
        private readonly PasswordHasher _passwordHasher;
        private readonly IValidator<UsuarioLoginDto> _loginValidator;
        private readonly ICarritoCompraRepositorio _carritoRepo;
        private readonly IProductoTallaRepositorio _productoTallaRepo;

        public LoginService(
            IUsuarioRepositorio usuarioRepo,IMapper mapper,ILogger<LoginService> logger, JwtService utilidades, IValidator<UsuarioLoginDto> loginValidator, ICarritoCompraRepositorio carritoRepo , PasswordHasher passwordHasher , IProductoTallaRepositorio productoTallaRepo)
        {
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _logger = logger;
            _utilidades = utilidades;
            _productoTallaRepo= productoTallaRepo;
            _carritoRepo = carritoRepo;
            _loginValidator = loginValidator;
            _passwordHasher = passwordHasher;
        }

        // Lógica de migración de carrito de usuario invitado a usuario registrado
        private async Task MigrarCarritoAsync(int invitadoId, int usuarioRealId)
        {
            _logger.LogInformation("🔄 Migrando carrito desde Invitado {0} → Usuario {1}", invitadoId, usuarioRealId);

            // Traer todos los ítems del invitado (sin tracking)
            var carritoInvitado = await _carritoRepo.ObtenerTodos(c => c.UsuarioId == invitadoId && c.Estado == true);

            if (!carritoInvitado.Any())
            {
                _logger.LogInformation("ℹ️ El invitado NO tenía productos en carrito.");
                return;
            }

            foreach (var item in carritoInvitado)
            {
                // Verificar si el producto ya existe en el usuario real
                var existe = await _carritoRepo.Obtener(c =>
                    c.UsuarioId == usuarioRealId &&
                    c.ProductoTallaId == item.ProductoTallaId &&
                    c.Estado == true
                );

                if (existe != null)
                {
                    // Sumar cantidad y actualizar subtotales
                    existe.Cantidad += item.Cantidad;
                    existe.SubTotal = existe.Cantidad * existe.PrecioUnitario;
                    await _carritoRepo.Actualizar(existe);
                }
                else
                {
                    // Crear nuevo item para usuario real
                    var nuevoItem = new CarritoCompra
                    {
                        UsuarioId = usuarioRealId,
                        ProductoTallaId = item.ProductoTallaId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario,
                        SubTotal = item.Cantidad * item.PrecioUnitario,
                        Estado = true
                    };

                    await _carritoRepo.Crear(nuevoItem);
                }

                // Eliminar el item del invitado (sin afectar stock)
                await _carritoRepo.Remover(item);
            }

            decimal totalCarrito = await _carritoRepo.CalcularTotalAsync(usuarioRealId);
            _logger.LogInformation("✅ Carrito migrado correctamente. Total actualizado: {0}", totalCarrito);
        }




        // ==============================
        // LOGIN DE USUARIO NORMAL
        // ==============================
        public async Task<ApiResponse<LoginResultDto>> LoginAsync(UsuarioLoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("🟡 Intentando login para: {Correo}", loginDto?.CorreoElectronico);

                var validation = await _loginValidator.ValidateAsync(loginDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<LoginResultDto>(validation.Errors);

                if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.CorreoElectronico) || string.IsNullOrWhiteSpace(loginDto.Contraseña))
                    return ResponseHelper.Fail<LoginResultDto>("Correo y contraseña son obligatorios.");

                var usuario = await _usuarioRepo.Obtener(
                    u => u.CorreoElectronico == loginDto.CorreoElectronico && u.Estado,
                    include: q => q.Include(x => x.UserRoles)
                                   .ThenInclude(ur => ur.Rol)
                                   .ThenInclude(r => r.PermRoles)
                                   .ThenInclude(rp => rp.Permiso)
                );

                if (usuario == null)
                    return ResponseHelper.Fail<LoginResultDto>("Usuario no encontrado o inactivo.", "CorreoElectronico", HttpStatusCode.Unauthorized);

                if (!_passwordHasher.VerificarPassword(loginDto.Contraseña, usuario.Contraseña))
                    return ResponseHelper.Fail<LoginResultDto>("Contraseña incorrecta.", "Contraseña", HttpStatusCode.Unauthorized);

                // Migrar carrito de invitado si existe
                int invitadoId = await _carritoRepo.ObtenerIdInvitadoActualAsync();
                if (invitadoId != 0 && invitadoId != usuario.UsuarioId)
                {
                    await MigrarCarritoAsync(invitadoId, usuario.UsuarioId);
                }

                // Generar JWT
                string token = _utilidades.GenerarJWT(usuario.UsuarioId);

                // Roles y permisos
                var roles = usuario.UserRoles
                    .Where(ur => ur.Estado && ur.Rol.Estado)
                    .Select(ur => ur.Rol.NombreRol)
                    .ToList();

                var permisos = usuario.UserRoles
                    .SelectMany(ur => ur.Rol.PermRoles)
                    .Where(rp => rp.Estado && rp.Permiso.Estado)
                    .Select(rp => rp.Permiso.NombrePermiso)
                    .Distinct()
                    .ToList();

                var resultado = new LoginResultDto
                {
                    Token = token,
                    Usuario = new UsuarioInfoDto
                    {
                        UsuarioId = usuario.UsuarioId,
                        NombreCompleto = usuario.NombreCompleto,
                        ApellidoCompleto = usuario.ApellidoCompleto,
                        DNI = usuario.DNI,
                        CorreoElectronico = usuario.CorreoElectronico,
                        Imagen = usuario.Imagen,
                        Roles = roles,
                        Permisos = permisos
                    }
                };

                _logger.LogInformation("✅ Login exitoso para usuario: {Correo}", loginDto.CorreoElectronico);
                return ResponseHelper.Success(resultado, "Login exitoso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante login de usuario.");
                return ResponseHelper.FailException<LoginResultDto>(ex);
            }
        }

        // ==============================
        // LOGIN DE INVITADO
        // ==============================
        public async Task<ApiResponse<LoginResultDto>> LoginInvitadoAsync()
        {
            try
            {
                _logger.LogInformation("🟡 Intentando login de invitado.");

                var usuarioInvitado = await _usuarioRepo.Obtener(
                    u => u.Estado && u.UserRoles.Any(ur => ur.Rol.NombreRol == "Invitado"),
                    include: q => q.Include(x => x.UserRoles)
                                   .ThenInclude(ur => ur.Rol)
                                   .ThenInclude(r => r.PermRoles)
                                   .ThenInclude(rp => rp.Permiso)
                );

                if (usuarioInvitado == null)
                    return ResponseHelper.Fail<LoginResultDto>("Usuario invitado no encontrado.", "UsuarioId", HttpStatusCode.NotFound);

                string token = _utilidades.GenerarJWT(usuarioInvitado.UsuarioId);

                var roles = usuarioInvitado.UserRoles
                    .Where(ur => ur.Estado && ur.Rol.Estado)
                    .Select(ur => ur.Rol.NombreRol)
                    .ToList();

                var permisos = usuarioInvitado.UserRoles
                    .SelectMany(ur => ur.Rol.PermRoles)
                    .Where(rp => rp.Estado && rp.Permiso.Estado)
                    .Select(rp => rp.Permiso.NombrePermiso)
                    .Distinct()
                    .ToList();

                var resultado = new LoginResultDto
                {
                    Token = token,
                    Usuario = new UsuarioInfoDto
                    {
                        UsuarioId = usuarioInvitado.UsuarioId,
                        NombreCompleto = usuarioInvitado.NombreCompleto,
                        ApellidoCompleto = usuarioInvitado.ApellidoCompleto,
                        DNI = usuarioInvitado.DNI,
                        CorreoElectronico = usuarioInvitado.CorreoElectronico,
                        Imagen = usuarioInvitado.Imagen,
                        Roles = roles,
                        Permisos = permisos
                    }
                };

                _logger.LogInformation("✅ Login de invitado exitoso (UsuarioId: {Id})", usuarioInvitado.UsuarioId);
                return ResponseHelper.Success(resultado, "Login de invitado exitoso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error durante login de invitado.");
                return ResponseHelper.FailException<LoginResultDto>(ex);
            }
        }
    }
}