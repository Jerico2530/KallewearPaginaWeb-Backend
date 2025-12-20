using ApiRopa.Models;
using ApiRopa.Models.Responses;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Pago;
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
 * PagoService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con los pagos de órdenes.
 * Funcionalidades clave:
 * - Obtener todos los pagos o un pago específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar pagos.
 * - Exportar listado de pagos a Excel.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de pagos, garantizando integridad y consistencia:
 * - Validación de datos de entrada.
 * - Evitar inconsistencias en la creación o actualización de pagos.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones de pago se realicen correctamente, manteniendo
 * el código limpio, profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa;

public class PagoService : IPagoService
{
    private readonly IPagoRepositorio _PagoRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<PagoService> _logger;
    private readonly IValidator<PagoCreateDto> _createValidator;
    private readonly IValidator<PagoUpdateDto> _updateValidator;
    private readonly IValidator<PagoUpdateDto> _patchValidator;
    private readonly IValidator<int> _getValidator;
    private readonly IValidator<int> _deleteValidator;
    private readonly AppDbContext _context;
    private readonly ExcelGenericoService _excelGenericoService;


    public PagoService(IPagoRepositorio PagoRepo, IMapper mapper, ILogger<PagoService> logger, IValidator<PagoCreateDto> createValidator, IValidator<PagoUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<PagoUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService
)
    {
        _PagoRepo = PagoRepo;
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

    public async Task<ApiResponse<List<PagoDto>>>  ObtenerTodosLosPagoAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Obteniendo todos los Pagos activos...");

            var Pagos = await _PagoRepo.ObtenerPagosConDetalles();

            if (Pagos == null || !Pagos.Any())
            {
                _logger.LogWarning("⚠️ No se encontraron Pagos registrados.");
                return ResponseHelper.Fail<List<PagoDto>>(
                    new List<ErrorDetail> { new() { Campo = "Pagos", Mensaje = "No se encontraron Pagos registrados." } },
                    HttpStatusCode.NotFound
                );
            }

            var PagosDto = _mapper.Map<IEnumerable<PagoDto>>(Pagos).ToList();

            _logger.LogInformation("✅ Se obtuvieron {Count} Pagos.", PagosDto.Count);
            return ResponseHelper.Success(PagosDto, "Pagos obtenidos exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Pagos.");
            return ResponseHelper.FailException<List<PagoDto>>(ex);
        }
    }

    public async Task<ApiResponse<PagoDto>> ObtenerPagoPorIdAsync(int id)
    {
        try
        {
            var validation = await _getValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            var Pago = await _PagoRepo.Obtener(a => a.PagoId == id);
            if (Pago == null)
            {
                _logger.LogWarning("⚠️ No se encontró el Pago con ID {Id}.", id);
                return ResponseHelper.Fail<PagoDto>(
                    new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Pago con ID {id}." } },
                    HttpStatusCode.NotFound
                );
            }

            var dto = _mapper.Map<PagoDto>(Pago);
            _logger.LogInformation("✅ Pago con ID {Id} obtenido correctamente.", id);
            return ResponseHelper.Success(dto, "Pago encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al obtener Pago por ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<byte[]>> ExportarExcelPagosAsync()
    {
        try
        {
            var pagos = await _context.Pagos.ToListAsync();
            var pagosDto = _mapper.Map<List<PagoDto>>(pagos);


            var bytes = await _excelGenericoService.ExportarExcel(
                pagosDto, "Reporte de Pagos", "Pagos"
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


    public async Task<ApiResponse<PagoDto>> CrearPagoAsync(PagoCreateDto createDto)
    {
        try
        {
            if (createDto == null)
                return ResponseHelper.Fail<PagoDto>("Datos inválidos para crear Pago.", "Pago");

            var validation = await _createValidator.ValidateAsync(createDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            var modelo = _mapper.Map<Pago>(createDto);
            await _PagoRepo.Crear(modelo);

            var dto = _mapper.Map<PagoDto>(modelo);
            _logger.LogInformation("✅ Pago '{Titulo}' creado correctamente.", dto.OrdenId);
            return ResponseHelper.Success(dto, "Pago creado correctamente", HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al crear Pago.");
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<object>> EliminarPagoAsync(int id)
    {
        try
        {
            var validation = await _deleteValidator.ValidateAsync(id);
            if (!validation.IsValid)
                return ResponseHelper.Fail<object>(validation.Errors);

            var Pago = await _PagoRepo.Obtener(a => a.PagoId == id);
            if (Pago == null)
                return ResponseHelper.Fail<object>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            await _PagoRepo.Remover(Pago);
            _logger.LogInformation("✅ Pago ID {Id} eliminado correctamente.", id);
            return ResponseHelper.Success<object>(null, "Pago eliminado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al eliminar Pago ID {Id}", id);
            return ResponseHelper.FailException<object>(ex);
        }
    }


    public async Task<ApiResponse<PagoDto>> ActualizarPagoAsync(int id, PagoUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return ResponseHelper.Fail<PagoDto>("Datos inválidos para actualizar Pago.", "Pago");

            var PagoExistente = await _PagoRepo.Obtener(a => a.PagoId == id, tracked: true);
            if (PagoExistente == null)
                return ResponseHelper.Fail<PagoDto>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            var validation = await _updateValidator.ValidateAsync(updateDto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            _mapper.Map(updateDto, PagoExistente);
            await _PagoRepo.ActualizarPago(PagoExistente);

            _logger.LogInformation("✅ Pago ID {Id} actualizado correctamente.", id);
            return ResponseHelper.Success<PagoDto>(null, "Pago actualizado correctamente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al actualizar Pago ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }

    public async Task<ApiResponse<PagoDto>> ActualizarParcialPagoAsync(int id, JsonPatchDocument<PagoUpdateDto> patchDto)
    {
        try
        {
            if (patchDto == null || id <= 0)
                return ResponseHelper.Fail<PagoDto>("Datos inválidos para la actualización parcial.", "Patch");

            var PagoExistente = await _PagoRepo.Obtener(a => a.PagoId == id, tracked: true);
            if (PagoExistente == null)
                return ResponseHelper.Fail<PagoDto>("Pago no encontrado.", "Id", HttpStatusCode.NotFound);

            var dto = _mapper.Map<PagoUpdateDto>(PagoExistente);
            patchDto.ApplyTo(dto);

            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return ResponseHelper.Fail<PagoDto>(validation.Errors);

            _mapper.Map(dto, PagoExistente);
            await _PagoRepo.ActualizarPago(PagoExistente);

            _logger.LogInformation("✅ PATCH aplicado correctamente al Pago ID {Id}.", id);
            return ResponseHelper.Success<PagoDto>(null, "Pago actualizado parcialmente", HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al aplicar PATCH al Pago ID {Id}", id);
            return ResponseHelper.FailException<PagoDto>(ex);
        }
    }



 

}



