using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Domain.Dto.OrdenDetalle;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
/*
 * OrdenDetalleService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con los detalles de órdenes.
 * Funcionalidades clave:
 * - Obtener todos los detalles de órdenes o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar detalles de órdenes.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios de mapeo.
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de los detalles de órdenes, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar inconsistencias en creación y mantener coherencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando operaciones correctas y manteniendo el código limpio, profesional y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class OrdenDetalleService : IOrdenDetalleService
{
    private readonly IOrdenDetalleRepositorio _OrdenDetalleRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdenDetalleService> _logger;
    private readonly IValidator<OrdenDetalleCreateDto> _createValidator;
    private readonly IValidator<OrdenDetalleUpdateDto> _updateValidator;
    private readonly IValidator<OrdenDetalleUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;


    public OrdenDetalleService(IOrdenDetalleRepositorio OrdenDetalleRepo, IMapper mapper, ILogger<OrdenDetalleService> logger, IValidator<OrdenDetalleCreateDto> createValidator, IValidator<OrdenDetalleUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<OrdenDetalleUpdateDto> patchValidator)
    {
        _OrdenDetalleRepo = OrdenDetalleRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _patchValidator = patchValidator;

    }

    public async Task<ApiResponse<List<OrdenDetalleDto>>> ObtenerTodosLosOrdenDetalleAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los OrdenDetalles activos...");
            // Obtener todos los detalles de órdenes desde el repositorio
            var OrdenDetalles = await _OrdenDetalleRepo.ObtenerTodo();
            // Validar si existen registros
            if (OrdenDetalles == null || !OrdenDetalles.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron OrdenDetalles registrados.");
                return ResponseHelper.Fail<List<OrdenDetalleDto>>(
                    new List<ErrorDetail> { new() { Campo = "OrdenDetalles", Mensaje = "No se encontraron OrdenDetalles registrados." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidades a DTO y ordenar por número de orden
            var OrdenDetallesDto = _mapper.Map<IEnumerable<OrdenDetalleDto>>(OrdenDetalles).OrderBy(a => a.Orden).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} OrdenDetalles.", OrdenDetallesDto.Count);
            return ResponseHelper.Success(OrdenDetallesDto, "OrdenDetalles obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener OrdenDetalles.");
            return ResponseHelper.FailException<List<OrdenDetalleDto>>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDetalleDto>> ObtenerOrdenDetallePorIdAsync(int id)
    {
        try
        {
            // Validar ID de entrada
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDetalleDto>(validation.Errors);
            // Obtener detalle de orden por ID
            var OrdenDetalle = await _OrdenDetalleRepo.Obtener(a => a.OrdenDetalleId == id);
            if (OrdenDetalle == null)
            {
                _logger.LogWarning("⚠️ No se encontró el OrdenDetalle con ID {Id}.", id);
                return ResponseHelper.Fail<OrdenDetalleDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el OrdenDetalle con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidad a DTO
            var dto = _mapper.Map<OrdenDetalleDto>(OrdenDetalle);
            _logger.LogInformation("✅ OrdenDetalle con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "OrdenDetalle encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener OrdenDetalle por ID {Id}", id);
            return ResponseHelper.FailException<OrdenDetalleDto>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDetalleDto>> CrearOrdenDetalleAsync(OrdenDetalleCreateDto createDto)
    {
        try
        {
            // Validar DTO de creación
            if (createDto == null)
                return ResponseHelper.Fail<OrdenDetalleDto>("Datos inválidos para crear OrdenDetalle.", "OrdenDetalle");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDetalleDto>(validation.Errors);
            // Mapear DTO a entidad y guardar en repositorio
            var modelo = _mapper.Map<OrdenDetalle>(createDto);
            await _OrdenDetalleRepo.Crear(modelo);

            var dto = _mapper.Map<OrdenDetalleDto>(modelo);
            _logger.LogInformation("✅ OrdenDetalle '{Titulo}' creado correctamente.", dto.OrdenId);
            return ResponseHelper.Success(dto, "OrdenDetalle creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear OrdenDetalle.");
            return ResponseHelper.FailException<OrdenDetalleDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarOrdenDetalleAsync(int id)
    {
        try
        {
            // Validar ID antes de eliminar
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);
            // Obtener entidad a eliminar
            var OrdenDetalle = await _OrdenDetalleRepo.Obtener(a => a.OrdenDetalleId == id);
            if (OrdenDetalle == null)
                return ResponseHelper.Fail<object>("OrdenDetalle no encontrado.", "Id", HttpStatusCode.NotFound);
            // Eliminar entidad
            await _OrdenDetalleRepo.Remover(OrdenDetalle);
            _logger.LogInformation("✅ OrdenDetalle ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "OrdenDetalle eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar OrdenDetalle ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDetalleDto>> ActualizarOrdenDetalleAsync(int id, OrdenDetalleUpdateDto updateDto)
    {
        try
        {
            // Validar DTO de actualización
            if (updateDto == null)
                return ResponseHelper.Fail<OrdenDetalleDto>("Datos inválidos para actualizar OrdenDetalle.", "OrdenDetalle");
            // Obtener entidad existente con tracking
            var OrdenDetalleExistente = await _OrdenDetalleRepo.Obtener(a => a.OrdenDetalleId == id, tracked: true);
            if (OrdenDetalleExistente == null)
                return ResponseHelper.Fail<OrdenDetalleDto>("OrdenDetalle no encontrado.", "Id", HttpStatusCode.NotFound);
            // Validar DTO de actualización
            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDetalleDto>(validation.Errors);
            // Mapear cambios al modelo existente y guardar
            _mapper.Map(updateDto, OrdenDetalleExistente);
            await _OrdenDetalleRepo.ActualizarOrdenDetalle(OrdenDetalleExistente);

            _logger.LogInformation("✅ OrdenDetalle ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<OrdenDetalleDto>(null, "OrdenDetalle actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar OrdenDetalle ID {Id}", id);
            return ResponseHelper.FailException<OrdenDetalleDto>(ex);
        }
    }

    public async Task<ApiResponse<OrdenDetalleDto>> ActualizarParcialOrdenDetalleAsync(int id, JsonPatchDocument<OrdenDetalleUpdateDto> patchDto)
    {
        try
        {
            // Validar DTO de patch
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<OrdenDetalleDto>("Datos inválidos para la actualización parcial.", "Patch");
            // Obtener entidad existente con tracking
            var OrdenDetalleExistente = await _OrdenDetalleRepo.Obtener(a => a.OrdenDetalleId == id, tracked: true);
            if (OrdenDetalleExistente == null)
                return ResponseHelper.Fail<OrdenDetalleDto>("OrdenDetalle no encontrado.", "Id", HttpStatusCode.NotFound);
            // Mapear entidad a DTO y aplicar patch
            var dto = _mapper.Map<OrdenDetalleUpdateDto>(OrdenDetalleExistente);
            patchDto.ApplyTo(dto);
            // Validar DTO modificado
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<OrdenDetalleDto>(validation.Errors);
            // Mapear cambios y actualizar entidad
            _mapper.Map(dto, OrdenDetalleExistente);
            await _OrdenDetalleRepo.ActualizarOrdenDetalle(OrdenDetalleExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al OrdenDetalle ID {Id}.", id);
            return ResponseHelper.Success<OrdenDetalleDto>(null, "OrdenDetalle actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al OrdenDetalle ID {Id}", id);
            return ResponseHelper.FailException<OrdenDetalleDto>(ex);
        }

    }
}
