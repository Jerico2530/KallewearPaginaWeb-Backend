using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.TipoPago;
using BiblotecaWeb.Model;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
/*
 * TipoPagoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con tipos de pago.
 * Funcionalidades clave:
 * - Obtener todos los tipos de pago o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar tipos de pago.
 * - Exportar listado de tipos de pago a Excel, excluyendo información sensible.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de tipos de pago, asegurando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * garantizando que las operaciones se realicen correctamente y manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */


namespace ApiRopa;

public class TipoPagoService : ITipoPagoService
{
    private readonly ITipoPagoRepositorio _TipoPagoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<TipoPagoService> _logger;
    private readonly IValidator<TipoPagoCreateDto> _createValidator;
    private readonly IValidator<TipoPagoUpdateDto> _updateValidator;
    private readonly IValidator<TipoPagoUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public TipoPagoService(ITipoPagoRepositorio TipoPagoRepo, IMapper mapper, ILogger<TipoPagoService> logger , IValidator<TipoPagoCreateDto> createValidator, IValidator<TipoPagoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<TipoPagoUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
    {
        _TipoPagoRepo = TipoPagoRepo;
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

    public async Task<ApiResponse<List<TipoPagoDto>>> ObtenerTodosLosTipoPagoAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los TipoPagos activos...");

            var TipoPagos = await _TipoPagoRepo.ObtenerTodo();

            if (TipoPagos == null || !TipoPagos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron TipoPagos registrados.");
                return ResponseHelper.Fail<List<TipoPagoDto>>(
                    new List<ErrorDetail> { new() { Campo = "TipoPagos", Mensaje = "No se encontraron TipoPagos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var TipoPagosDto = _mapper.Map<IEnumerable<TipoPagoDto>>(TipoPagos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} TipoPagos.", TipoPagosDto.Count);
            return ResponseHelper.Success(TipoPagosDto, "TipoPagos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener TipoPagos.");
            return ResponseHelper.FailException<List<TipoPagoDto>>(ex);
        }
    }

    public async Task<ApiResponse<TipoPagoDto>> ObtenerTipoPagoPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TipoPagoDto>(validation.Errors);

            var TipoPago = await _TipoPagoRepo.Obtener(a => a.TipoPagoId == id);
            if (TipoPago == null)
            {
                _logger.LogWarning("⚠️ No se encontró el TipoPago con ID {Id}.", id);
                return ResponseHelper.Fail<TipoPagoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el TipoPago con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<TipoPagoDto>(TipoPago);
            _logger.LogInformation("✅ TipoPago con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "TipoPago encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener TipoPago por ID {Id}", id);
            return ResponseHelper.FailException<TipoPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelTipoPagosAsync()
    {
        try
        {
            var tipoPagos = await _context.TipoPagos.ToListAsync();
            var tipoPagosDto = _mapper.Map<List<TipoPagoDto>>(tipoPagos);

            // Excluir propiedades sensibles o imágenes
            var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

            var bytes = await _excelGenericoService.ExportarExcel(
                tipoPagosDto, "Reporte de TipoPagos", "TipoPagos", excluir
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


    public async Task<ApiResponse<TipoPagoDto>> CrearTipoPagoAsync(TipoPagoCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<TipoPagoDto>("Datos inválidos para crear TipoPago.", "TipoPago");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TipoPagoDto>(validation.Errors);

 

            var modelo = _mapper.Map<TipoPago>(createDto);
            await _TipoPagoRepo.Crear(modelo);

            var dto = _mapper.Map<TipoPagoDto>(modelo);
            _logger.LogInformation("✅ TipoPago '{DescripcionTipoPago}' creado correctamente.", dto.DescripcionTipoPago);
            return ResponseHelper.Success(dto, "TipoPago creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear TipoPago.");
            return ResponseHelper.FailException<TipoPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarTipoPagoAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var TipoPago = await _TipoPagoRepo.Obtener(a => a.TipoPagoId == id);
            if (TipoPago == null)
                return ResponseHelper.Fail<object>("TipoPago no encontrado.", "Id", HttpStatusCode.NotFound);

            await _TipoPagoRepo.Remover(TipoPago);
            _logger.LogInformation("✅ TipoPago ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "TipoPago eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar TipoPago ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }

    public async Task<ApiResponse<TipoPagoDto>> ActualizarTipoPagoAsync(int id, TipoPagoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<TipoPagoDto>("Datos inválidos para actualizar TipoPago.", "TipoPago");

            var TipoPagoExistente = await _TipoPagoRepo.Obtener(a => a.TipoPagoId == id, tracked: true);
            if (TipoPagoExistente == null)
                return ResponseHelper.Fail<TipoPagoDto>("TipoPago no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TipoPagoDto>(validation.Errors);

            _mapper.Map(updateDto, TipoPagoExistente);
            await _TipoPagoRepo.ActualizarTipoPago(TipoPagoExistente);

            _logger.LogInformation("✅ TipoPago ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<TipoPagoDto>(null, "TipoPago actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar TipoPago ID {Id}", id);
            return ResponseHelper.FailException<TipoPagoDto>(ex);
        }
    }

    public async Task<ApiResponse<TipoPagoDto>> ActualizarParcialTipoPagoAsync(int id, JsonPatchDocument<TipoPagoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<TipoPagoDto>("Datos inválidos para la actualización parcial.", "Patch");

            var TipoPagoExistente = await _TipoPagoRepo.Obtener(a => a.TipoPagoId == id, tracked: true);
            if (TipoPagoExistente == null)
                return ResponseHelper.Fail<TipoPagoDto>("TipoPago no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<TipoPagoUpdateDto>(TipoPagoExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<TipoPagoDto>(validation.Errors);

            _mapper.Map(dto, TipoPagoExistente);
            await _TipoPagoRepo.ActualizarTipoPago(TipoPagoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al TipoPago ID {Id}.", id);
            return ResponseHelper.Success<TipoPagoDto>(null, "TipoPago actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al TipoPago ID {Id}", id);
            return ResponseHelper.FailException<TipoPagoDto>(ex);
        }
    }
}
