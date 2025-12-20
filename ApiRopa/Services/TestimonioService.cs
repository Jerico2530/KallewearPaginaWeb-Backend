using ApiRopa.Models;
using ApiRopa.Models.Responses;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Services.IServices;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Domain.Dto.Testimonio;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Model.Dto;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
/*
 * TestimonioService
 *
 * Servicio encargado de gestionar la lógica de negocio relacionada con testimonios de clientes.
 * Funcionalidades clave:
 * - Obtener todos los testimonios o uno específico por ID.
 * - Crear, actualizar (completo o parcial) y eliminar testimonios.
 * - Exportar listado de testimonios a Excel, excluyendo información sensible.
 * - Validar datos mediante FluentValidation.
 * - Interactuar con repositorios especializados y servicios auxiliares (Excel, mapeo).
 *
 * Propósito del componente:
 * Centralizar la lógica de negocio de testimonios, garantizando integridad y consistencia:
 * - Validación de datos antes de operaciones críticas.
 * - Evitar duplicados y mantener consistencia en actualizaciones.
 *
 * Este servicio actúa como capa intermedia entre controladores y repositorios,
 * asegurando que las operaciones se realicen correctamente, manteniendo el código limpio,
 * profesional y desacoplado de la capa de datos.
 */

namespace ApiRopa.Services
{
    public class TestimonioService : ITestimonioService
    {
        private readonly ITestimonioRepositorio _TestimonioRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<TestimonioService> _logger;
        private readonly IValidator<TestimonioCreateDto> _createValidator;
        private readonly IValidator<TestimonioUpdateDto> _updateValidator;
        private readonly IValidator<TestimonioUpdateDto> _patchValidator;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;
        private readonly AppDbContext _context;
        private readonly ExcelGenericoService _excelGenericoService;


        public TestimonioService(ITestimonioRepositorio TestimonioRepo, IMapper mapper, ILogger<TestimonioService> logger , IValidator<TestimonioCreateDto> createValidator, IValidator<TestimonioUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator, IValidator<TestimonioUpdateDto> patchValidator, AppDbContext context, ExcelGenericoService excelGenericoService)
        {
            _TestimonioRepo = TestimonioRepo;
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

        public async Task<ApiResponse<List<TestimonioDto>>> ObtenerTodosLosTestimonioAsync()
        {

            try
            {
                _logger.LogInformation("🔍 Obteniendo todos los Testimonios activos...");

                var Testimonios = await _TestimonioRepo.ObtenerTestimoniosConDetalles();

                if (Testimonios == null || !Testimonios.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron Testimonios registrados.");
                    return ResponseHelper.Fail<List<TestimonioDto>>(
                        new List<ErrorDetail> { new() { Campo = "Testimonios", Mensaje = "No se encontraron Testimonios registrados." } },
                        HttpStatusCode.NotFound
                    );
                }

                var TestimoniosDto = _mapper.Map<IEnumerable<TestimonioDto>>(Testimonios).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} Testimonios.", TestimoniosDto.Count);
                return ResponseHelper.Success(TestimoniosDto, "Testimonios obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Testimonios.");
                return ResponseHelper.FailException<List<TestimonioDto>>(ex);
            }
        }

        public async Task<ApiResponse<TestimonioDto>> ObtenerTestimonioPorIdAsync(int id)
        {
            try
            {
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<TestimonioDto>(validation.Errors);

                var Testimonio = await _TestimonioRepo.Obtener(a => a.TestimonioId == id);
                if (Testimonio == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el Testimonio con ID {Id}.", id);
                    return ResponseHelper.Fail<TestimonioDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el Testimonio con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<TestimonioDto>(Testimonio);
                _logger.LogInformation("✅ Testimonio con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Testimonio encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener Testimonio por ID {Id}", id);
                return ResponseHelper.FailException<TestimonioDto>(ex);
            }
        }

        public async Task<ApiResponse<byte[]>> ExportarExcelTestimoniosAsync()
        {
            try
            {
                var testimonios = await _TestimonioRepo.ObtenerTestimoniosConDetalles();
                var testimoniosDto = _mapper.Map<List<TestimonioDto>>(testimonios);

                // Excluir propiedades sensibles o imágenes
                var excluir = new[] { "Contraseña", "ContraseñaVisible", "Imagen" };

                var bytes = await _excelGenericoService.ExportarExcel(
                    testimoniosDto, "Reporte de Testimonios", "Testimonios", excluir
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


        public async Task<ApiResponse<TestimonioDto>> CrearTestimonioAsync(TestimonioCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<TestimonioDto>("Datos inválidos para crear Testimonio.", "Testimonio");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<TestimonioDto>(validation.Errors);


                var modelo = _mapper.Map<Testimonio>(createDto);
                await _TestimonioRepo.Crear(modelo);

                var dto = _mapper.Map<TestimonioDto>(modelo);
                _logger.LogInformation("✅ Testimonio '{Descripcion}' creado correctamente.", dto.Descripcion);
                return ResponseHelper.Success(dto, "Testimonio creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear Testimonio.");
                return ResponseHelper.FailException<TestimonioDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarTestimonioAsync(int id)
        {
            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var Testimonio = await _TestimonioRepo.Obtener(a => a.TestimonioId == id);
                if (Testimonio == null)
                    return ResponseHelper.Fail<object>("Testimonio no encontrado.", "Id", HttpStatusCode.NotFound);

                await _TestimonioRepo.Remover(Testimonio);
                _logger.LogInformation("✅ Testimonio ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Testimonio eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar Testimonio ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }

        public async Task<ApiResponse<TestimonioDto>> ActualizarTestimonioAsync(int id, TestimonioUpdateDto updateDto)
        {

            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<TestimonioDto>("Datos inválidos para actualizar Testimonio.", "Testimonio");

                var TestimonioExistente = await _TestimonioRepo.Obtener(a => a.TestimonioId == id, tracked: true);
                if (TestimonioExistente == null)
                    return ResponseHelper.Fail<TestimonioDto>("Testimonio no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<TestimonioDto>(validation.Errors);

                _mapper.Map(updateDto, TestimonioExistente);
                await _TestimonioRepo.ActualizarTestimonio(TestimonioExistente);

                _logger.LogInformation("✅ Testimonio ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<TestimonioDto>(null, "Testimonio actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar Testimonio ID {Id}", id);
                return ResponseHelper.FailException<TestimonioDto>(ex);
            }
        }

        public async Task<ApiResponse<TestimonioDto>> ActualizarParcialTestimonioAsync(int id, JsonPatchDocument<TestimonioUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<TestimonioDto>("Datos inválidos para la actualización parcial.", "Patch");

                var TestimonioExistente = await _TestimonioRepo.Obtener(a => a.TestimonioId == id, tracked: true);
                if (TestimonioExistente == null)
                    return ResponseHelper.Fail<TestimonioDto>("Testimonio no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<TestimonioUpdateDto>(TestimonioExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<TestimonioDto>(validation.Errors);

                _mapper.Map(dto, TestimonioExistente);
                await _TestimonioRepo.ActualizarTestimonio(TestimonioExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al Testimonio ID {Id}.", id);
                return ResponseHelper.Success<TestimonioDto>(null, "Testimonio actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al Testimonio ID {Id}", id);
                return ResponseHelper.FailException<TestimonioDto>(ex);
            }
        }



    }

}
