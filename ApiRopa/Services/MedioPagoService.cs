using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Services;
using ApiRopa.Services.IServices;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.MedioPago;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * MedioPagoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con medios de pago.
 * Funcionalidades clave:
 * - Obtener todos los medios de pago o uno por ID.
 * - Crear, actualizar (completo o parcial) y eliminar medios de pago.
 * - Exportar listado de medios de pago a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de medios de pago, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar duplicados en creación y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */
namespace ApiRopa;

public class MedioPagoService : IMedioPagoService
{
    private readonly IMedioPagoRepositorio _MedioPagoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<MedioPagoService> _logger;
    private readonly IValidator<MedioPagoCreateDto> _createValidator;
    private readonly IValidator<MedioPagoUpdateDto> _updateValidator;
    private readonly IValidator<MedioPagoUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public MedioPagoService(IMedioPagoRepositorio MedioPagoRepo, IMapper mapper, ILogger<MedioPagoService> logger , IValidator<MedioPagoCreateDto> createValidator, IValidator<MedioPagoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<MedioPagoUpdateDto> patchValidator , AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _MedioPagoRepo = MedioPagoRepo;
        _mapper = mapper;
        _logger = logger;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _getValidator = getValidator;
        _deleteValidator = deleteValidator;
        _patchValidator = patchValidator;
        _context = context;
        _excelGenericoService = excelGenericoService;

    }

    public async Task<ApiResponse<List<MedioPagoDto>>> ObtenerTodosLosMedioPagoAsync()
    {

        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los MedioPagos activos...");
            // Obtener todos los medios de pago de la base de datos
            var MedioPagos = await _MedioPagoRepo.ObtenerTodo();

            if (MedioPagos == null || !MedioPagos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron MedioPagos registrados.");
                return ResponseHelper.Fail<List<MedioPagoDto>>(
                    new List<ErrorDetail> { new() { Campo = "MedioPagos", Mensaje = "No se encontraron MedioPagos registrados." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidades a DTO para retornar solo los datos necesarios
            var MedioPagosDto = _mapper.Map<IEnumerable<MedioPagoDto>>(MedioPagos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} MedioPagos.", MedioPagosDto.Count);
            return ResponseHelper.Success(MedioPagosDto, "MedioPagos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener MedioPagos.");
            return ResponseHelper.FailException<List<MedioPagoDto>>(ex);
        }
    }

    public async Task<ApiResponse<MedioPagoDto>> ObtenerMedioPagoPorIdAsync(int id)
    {
        try
        {
            // Validación de ID mediante FluentValidation
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MedioPagoDto>(validation.Errors);

            var MedioPago = await _MedioPagoRepo.Obtener(a => a.MedioPagoId == id);
            if (MedioPago == null)
            {
                _logger.LogWarning("⚠️ No se encontró el MedioPago con ID {Id}.", id);
                return ResponseHelper.Fail<MedioPagoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el MedioPago con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }
            // Mapear entidad a DTO
            var dto = _mapper.Map<MedioPagoDto>(MedioPago);
            _logger.LogInformation("✅ MedioPago con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "MedioPago encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener MedioPago por ID {Id}", id);
            return ResponseHelper.FailException<MedioPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelMedioPagosAsync()
    {
        try
        {
            // Obtener todos los medios de pago y mapear a DTO
            var medioPagos = await _context.MedioPagos.ToListAsync();
            var medioPagosDto = _mapper.Map<List<MedioPagoDto>>(medioPagos);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                medioPagosDto, "Reporte de MedioPagos", "MedioPagos", excluir
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


    public async Task<ApiResponse<MedioPagoDto>> CrearMedioPagoAsync(MedioPagoCreateDto createDto)
    {

        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<MedioPagoDto>("Datos inválidos para crear MedioPago.", "MedioPago");
            // Validar DTO de creación
            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MedioPagoDto>(validation.Errors);

            // Mapear DTO a entidad y guardar en base de datos
            var modelo = _mapper.Map<MedioPago>(createDto);
            await _MedioPagoRepo.Crear(modelo);

            var dto = _mapper.Map<MedioPagoDto>(modelo);
            _logger.LogInformation("✅ MedioPago '{DescripcionMedioPago}' creado correctamente.", dto.DescripcionMedioPago);
            return ResponseHelper.Success(dto, "MedioPago creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear MedioPago.");
            return ResponseHelper.FailException<MedioPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarMedioPagoAsync(int id)
    {
        try
        {
            // Validar ID antes de eliminar
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var MedioPago = await _MedioPagoRepo.Obtener(a => a.MedioPagoId == id);
            if (MedioPago == null)
                return ResponseHelper.Fail<object>("MedioPago no encontrado.", "Id", HttpStatusCode.NotFound);
            // Eliminar entidad
            await _MedioPagoRepo.Remover(MedioPago);
            _logger.LogInformation("✅ MedioPago ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "MedioPago eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar MedioPago ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<MedioPagoDto>> ActualizarMedioPagoAsync(int id, MedioPagoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<MedioPagoDto>("Datos inválidos para actualizar MedioPago.", "MedioPago");
            // Obtener entidad existente con tracking para actualizar
            var MedioPagoExistente = await _MedioPagoRepo.Obtener(a => a.MedioPagoId == id, tracked: true);
            if (MedioPagoExistente == null)
                return ResponseHelper.Fail<MedioPagoDto>("MedioPago no encontrado.", "Id", HttpStatusCode.NotFound);
            // Validar DTO de actualización
            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MedioPagoDto>(validation.Errors);
            // Mapear cambios al modelo existente y guardar
            _mapper.Map(updateDto, MedioPagoExistente);
            await _MedioPagoRepo.ActualizarMedioPago(MedioPagoExistente);

            _logger.LogInformation("✅ MedioPago ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<MedioPagoDto>(null, "MedioPago actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar MedioPago ID {Id}", id);
            return ResponseHelper.FailException<MedioPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<MedioPagoDto>> ActualizarParcialMedioPagoAsync(int id, JsonPatchDocument<MedioPagoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<MedioPagoDto>("Datos inválidos para la actualización parcial.", "Patch");
            // Obtener entidad existente con tracking
            var MedioPagoExistente = await _MedioPagoRepo.Obtener(a => a.MedioPagoId == id, tracked: true);
            if (MedioPagoExistente == null)
                return ResponseHelper.Fail<MedioPagoDto>("MedioPago no encontrado.", "Id", HttpStatusCode.NotFound);
            // Aplicar patch al DTO antes de mapear a entidad
            var dto = _mapper.Map<MedioPagoUpdateDto>(MedioPagoExistente);
            patchDto.ApplyTo(dto);
            // Validar DTO modificado
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<MedioPagoDto>(validation.Errors);
            // Mapear cambios y actualizar
            _mapper.Map(dto, MedioPagoExistente);
            await _MedioPagoRepo.ActualizarMedioPago(MedioPagoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al MedioPago ID {Id}.", id);
            return ResponseHelper.Success<MedioPagoDto>(null, "MedioPago actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al MedioPago ID {Id}", id);
            return ResponseHelper.FailException<MedioPagoDto>(ex);
        }
    }



}




