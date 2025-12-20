using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Servicios;
using AutoMapper;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Usuario;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
/*
 * UsuarioService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con los usuarios de la aplicación.
 * Funcionalidades clave:
 * - Obtener todos los usuarios o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar usuarios.
 * - Exportar listado de usuarios a Excel, excluyendo información sensible.
 * - Validar datos mediante FluentValidation.
 * - Gestionar roles de usuario automáticamente al crear un nuevo usuario.
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de usuarios, garantizando integridad y consistencia:
 * - Validación de datos de entrada antes de operaciones críticas.
 * - Evitar inconsistencias en la creación y actualización de usuarios.
 * - Mantener relaciones correctas con roles y otras entidades asociadas.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, con un código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepositorio _UsuarioRepo;
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    private readonly ILogger<UsuarioService> _logger;
    private readonly IValidator<UsuarioCreateDto> _createValidator;
    private readonly IValidator<UsuarioUpdateDto> _updateValidator;
    private readonly IValidator<UsuarioUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly ExcelGenericoService _excelGenericoService;



    public UsuarioService(IUsuarioRepositorio usuarioRepo, IMapper mapper, ILogger<UsuarioService> logger, AppDbContext context, IValidator<UsuarioCreateDto> createValidator, IValidator<UsuarioUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<UsuarioUpdateDto> patchValidator , ExcelGenericoService excelGenericoService )
    {
        _UsuarioRepo = usuarioRepo;
        _mapper = mapper;
        _logger = logger;
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _patchValidator = patchValidator;
        _excelGenericoService = excelGenericoService;

    }

    public async Task<ApiResponse<List<UsuarioDto>>>  ObtenerTodosLosUsuarioAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Usuarios activos...");

            var Usuarios = await _UsuarioRepo.ObtenerTodo();

            if (Usuarios == null || !Usuarios.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Usuarios registrados.");
                return ResponseHelper.Fail<List<UsuarioDto>>(
                    new List<ErrorDetail> { new() { Campo = "Usuarios", Mensaje = "No se encontraron Usuarios registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var UsuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(Usuarios).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Usuarios.", UsuariosDto.Count);
            return ResponseHelper.Success(UsuariosDto, "Usuarios obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Usuarios.");
            return ResponseHelper.FailException<List<UsuarioDto>>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelUsuariosAsync()
    {
        try
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var usuariosDto = _mapper.Map<List<UsuarioDto>>(usuarios);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                usuariosDto, "Reporte de Usuarios", "Usuarios", excluir
            );

            if (bytes == null || bytes.Length == 0)
                return ResponseHelper.Fail<byte[]>(
                    "No se generó ningún archivo Excel.",
                    campo: null,
                    code: HttpStatusCode.NotFound
                );

            return ResponseHelper.Success(bytes, "Excel generado correctamente.");
        }
        catch (Exception ex)
        {
            return ResponseHelper.FailException<byte[]>(ex);
        }
    }




    public async Task<ApiResponse<UsuarioDto>> ObtenerUsuarioPorIdAsync(int id)
    {


        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

            var Usuario = await _UsuarioRepo.Obtener(a => a.UsuarioId == id);
            if (Usuario == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Usuario con ID {Id}.", id);
                return ResponseHelper.Fail<UsuarioDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Usuario con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<UsuarioDto>(Usuario);
            _logger.LogInformation("✅ Usuario con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Usuario encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Usuario por ID {Id}", id);
            return ResponseHelper.FailException<UsuarioDto>(ex);
        }
    }

    public async Task<ApiResponse<UsuarioDto>> CrearUsuarioAsync(UsuarioCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para crear usuario.", "Usuario");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

            if (createDto.Contraseña != createDto.ContraseñaVisible)
                return ResponseHelper.Fail<UsuarioDto>("Las contraseñas no coinciden.", "Contraseña");

            var existe = await _UsuarioRepo.Obtener(u => u.DNI.ToLower() == createDto.DNI.ToLower());
            if (existe != null)
                return ResponseHelper.Fail<UsuarioDto>("Ya existe un usuario con ese DNI.", "DNI", HttpStatusCode.Conflict);

            createDto.Contraseña = BCrypt.Net.BCrypt.HashPassword(createDto.Contraseña, workFactor: 8);

            var usuario = _mapper.Map<Usuario>(createDto);

            await _UsuarioRepo.Crear(usuario);

            var rolUsuario = await _context.Roles.FirstOrDefaultAsync(r => r.NombreRol == "Usuario");

            if (rolUsuario == null)
            {
                // Si no existe, lo creamos automáticamente
                rolUsuario = new Rol
                {
                    NombreRol = "Usuario",
                    Estado = true
                };
                _context.Roles.Add(rolUsuario);
                await _context.SaveChangesAsync();
            }

            // 🔗 Crear relación UserRol
            var userRol = new UserRol
            {
                UsuarioId = usuario.UsuarioId,
                RolId = rolUsuario.RolId,
                Estado = true
            };

            _context.UserRoles.Add(userRol);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<UsuarioDto>(usuario); // ✅ dto ya tendrá UsuarioId

            return ResponseHelper.Success(dto, "Usuario creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            return ResponseHelper.FailException<UsuarioDto>(ex);
        }
    }


    public async Task<ApiResponse<object>> EliminarUsuarioAsync(int id)
    {
        try
        {

            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var usuario = await _UsuarioRepo.Obtener(p => p.UsuarioId == id);
            if (usuario == null)
                return ResponseHelper.Fail<object>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

            // Cargar relaciones
            await _context.Entry(usuario).Collection(u => u.UserRoles).LoadAsync();
            await _context.Entry(usuario).Collection(u => u.ProductoFavoritos).LoadAsync();
            await _context.Entry(usuario).Collection(u => u.Testimonios).LoadAsync();

            var relaciones = new List<string>();
            if (usuario.UserRoles.Any()) relaciones.Add("UserRoles");
            if (usuario.ProductoFavoritos.Any()) relaciones.Add("ProductoFavoritos");
            if (usuario.Testimonios.Any()) relaciones.Add("Testimonios");

            if (relaciones.Any())
            {
                var mensaje = $"No se puede eliminar el usuario porque está relacionado con: {string.Join(", ", relaciones)}";
                _logger.LogWarning(mensaje);
                return ResponseHelper.Fail<object>(mensaje, "Relaciones", HttpStatusCode.Conflict);
            }

            // Eliminar
            await _UsuarioRepo.Remover(usuario);

            _logger.LogInformation("✅ Usuario ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Usuario eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Usuario ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }



    public async Task<ApiResponse<UsuarioDto>> ActualizarUsuarioAsync(int id, UsuarioUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para actualizar usuario.", "Usuario");

            var usuarioExistente = await _UsuarioRepo.Obtener(u => u.UsuarioId == id, tracked: true);
            if (usuarioExistente == null)
                return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.OK);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

            _mapper.Map(updateDto, usuarioExistente);
            await _UsuarioRepo.ActualizarUsuario(usuarioExistente);

            _logger.LogInformation("✅ Usuario ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<UsuarioDto>(null, "Usuario actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar usuario ID {Id}", id);
            return ResponseHelper.FailException<UsuarioDto>(ex);
        }
    }



    public async Task<ApiResponse<UsuarioDto>> ObtenerUsuarioActualAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

            var usuario = await _UsuarioRepo.Obtener(u => u.UsuarioId == id && u.Estado);
            if (usuario == null)
                return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<UsuarioDto>(usuario);
            _logger.LogInformation("✅ Usuario con ID {Id} obtenido correctamente.", id);

            return ResponseHelper.Success(dto, "Usuario obtenido correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener usuario ID {Id}", id);
            return ResponseHelper.FailException<UsuarioDto>(ex);
        }
    }


    public async Task<ApiResponse<UsuarioDto>> ActualizarParcialUsuarioAsync(int id, JsonPatchDocument<UsuarioUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<UsuarioDto>("Datos inválidos para la actualización parcial.", "Patch");

            var UsuarioExistente = await _UsuarioRepo.Obtener(a => a.UsuarioId == id, tracked: true);
            if (UsuarioExistente == null)
                return ResponseHelper.Fail<UsuarioDto>("Usuario no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<UsuarioUpdateDto>(UsuarioExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<UsuarioDto>(validation.Errors);

            _mapper.Map(dto, UsuarioExistente);
            await _UsuarioRepo.ActualizarUsuario(UsuarioExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Usuario ID {Id}.", id);
            return ResponseHelper.Success<UsuarioDto>(null, "Usuario actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Usuario ID {Id}", id);
            return ResponseHelper.FailException<UsuarioDto>(ex);
        }
    }
}





