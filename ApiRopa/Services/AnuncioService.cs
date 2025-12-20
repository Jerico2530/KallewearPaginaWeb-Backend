using ApiRopa.Models;
using ApiRopa.Repositorio.IRepositorio;
using ApiRopa.Services.IServices;
using AutoMapper;
using BiblotecaWeb;
using BiblotecaWeb.Datos;
using BiblotecaWeb.Model.Dto;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using System.Net;
using BiblotecaWeb.Domain.Entities;
using BiblotecaWeb.Domain.Dto.Anuncio;
using ApiRopa.Models.Responses;

namespace ApiRopa.Services
{
    // Dependencias necesarias para operar la lógica de negocio
    public class AnuncioService : IAnuncioService
    {
        private readonly IAnuncioRepositorio _AnuncioRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<AnuncioService> _logger;
        private readonly IValidator<AnuncioCreateDto> _createValidator;
        private readonly IValidator<AnuncioUpdateDto> _updateValidator;
        private readonly IValidator<AnuncioUpdateDto> _patchValidator;
        private readonly AppDbContext _context;
        private readonly ExcelGenericoService _excelGenericoService;
        private readonly IValidator<int> _getValidator;
        private readonly IValidator<int> _deleteValidator;

        /// <summary>
        /// Constructor donde se inyectan las dependencias del servicio.
        /// Cada validator corresponde a un caso de uso específico para mantener integridad de datos.
        /// </summary>
        public AnuncioService(IAnuncioRepositorio AnuncioRepo, IMapper mapper, ILogger<AnuncioService> logger, IValidator<AnuncioCreateDto> createValidator, IValidator<AnuncioUpdateDto> updateValidator, IValidator<int> getValidator, IValidator<int> deleteValidator , IValidator<AnuncioUpdateDto> patchValidator,AppDbContext context ,ExcelGenericoService excelGenericoService)
        {
            _AnuncioRepo = AnuncioRepo;
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

        public async Task<ApiResponse<List<AnuncioDto>>> ObtenerTodosLosAnuncioAsync()
        {
            try
            {
                // Obtiene anuncios solo activos y validados
                _logger.LogInformation("🔍 Obteniendo todos los anuncios activos...");

                var anuncios = await _AnuncioRepo.ObtenerTodo();

                if (anuncios == null || !anuncios.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron anuncios registrados.");
                    return ResponseHelper.Fail<List<AnuncioDto>>(
                        new List<ErrorDetail> { new() { Campo = "Anuncios", Mensaje = "No se encontraron anuncios registrados." } },
                        HttpStatusCode.NotFound
                    );
                }
                // Ordenamiento por prioridad de visualización
                var anunciosDto = _mapper.Map<IEnumerable<AnuncioDto>>(anuncios).OrderBy(a => a.Orden).ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} anuncios.", anunciosDto.Count);
                return ResponseHelper.Success(anunciosDto, "Anuncios obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener anuncios.");
                return ResponseHelper.FailException<List<AnuncioDto>>(ex);
            }
        }

        public async Task<ApiResponse<List<AnuncioDto>>> ObtenerTodosLosAnunciosAdminAsync()
        {
            try
            {
                _logger.LogInformation("📢 Obteniendo todos los anuncios (modo administrador)...");
                // Devuelve todos los anuncios, incluyendo inactivos y de administración
                var anuncios = await _AnuncioRepo.ObtenerTodo();

                if (anuncios == null || !anuncios.Any())
                {
                    _logger.LogWarning("⚠️ No se encontraron anuncios en la base de datos.");
                    return ResponseHelper.Fail<List<AnuncioDto>>(
                        new List<ErrorDetail> { new() { Campo = "Anuncios", Mensaje = "No se encontraron anuncios registrados." } },
                        HttpStatusCode.NotFound
                    );
                }
                // Orden para vista de administración (por fecha de registro)
                var anunciosDto = _mapper
                    .Map<IEnumerable<AnuncioDto>>(anuncios)
                    .OrderByDescending(a => a.FechaRegistro)
                    .ToList();

                _logger.LogInformation("✅ Se obtuvieron {Count} anuncios en modo administrador.", anunciosDto.Count);
                return ResponseHelper.Success(anunciosDto, "Anuncios obtenidos exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener todos los anuncios (modo administrador).");
                return ResponseHelper.FailException<List<AnuncioDto>>(ex);
            }
        }


        public async Task<ApiResponse<AnuncioDto>> ObtenerAnuncioPorIdAsync(int id)
        {
            try
            {
                // Validación de valor recibido
                var validation = await _getValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<AnuncioDto>(validation.Errors);
                // Búsqueda del recurso solicitado
                var anuncio = await _AnuncioRepo.Obtener(a => a.AnuncioId == id);
                if (anuncio == null)
                {
                    _logger.LogWarning("⚠️ No se encontró el anuncio con ID {Id}.", id);
                    return ResponseHelper.Fail<AnuncioDto>(
                        new List<ErrorDetail> { new() { Campo = "Id", Mensaje = $"No se encontró el anuncio con ID {id}." } },
                        HttpStatusCode.NotFound
                    );
                }

                var dto = _mapper.Map<AnuncioDto>(anuncio);
                _logger.LogInformation("✅ Anuncio con ID {Id} obtenido correctamente.", id);
                return ResponseHelper.Success(dto, "Anuncio encontrado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al obtener anuncio por ID {Id}", id);
                return ResponseHelper.FailException<AnuncioDto>(ex);
            }
        }

        public async Task<ApiResponse<byte[]>> ExportarExcelAnunciosAsync()
        {
            try
            {
                // Obtiene todos los anuncios para exportación completa
                var anuncios = await _context.Anuncios.ToListAsync();
                var anunciosDto = _mapper.Map<List<AnuncioDto>>(anuncios);

                // Columnas a excluir del reporte Excel
                var excluir = new[] { "Imagen" };

                var bytes = await _excelGenericoService.ExportarExcel(
                    anunciosDto, "Reporte de anuncios", "anuncios", excluir
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


        public async Task<ApiResponse<AnuncioDto>> CrearAnuncioAsync(AnuncioCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                    return ResponseHelper.Fail<AnuncioDto>("Datos inválidos para crear anuncio.", "Anuncio");

                var validation = await _createValidator.ValidateAsync(createDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<AnuncioDto>(validation.Errors);

                var existe = await _AnuncioRepo.Obtener(a => a.Titulo.ToLower() == createDto.Titulo.ToLower());
                if (existe != null)
                    return ResponseHelper.Fail<AnuncioDto>("Ya existe un anuncio con ese título.", "Titulo", HttpStatusCode.Conflict);

                var modelo = _mapper.Map<Anuncio>(createDto);
                await _AnuncioRepo.Crear(modelo);

                var dto = _mapper.Map<AnuncioDto>(modelo);
                _logger.LogInformation("✅ Anuncio '{Titulo}' creado correctamente.", dto.Titulo);
                return ResponseHelper.Success(dto, "Anuncio creado correctamente", HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al crear anuncio.");
                return ResponseHelper.FailException<AnuncioDto>(ex);
            }
        }

        public async Task<ApiResponse<object>> EliminarAnuncioAsync(int id)
        {
            try
            {
                var validation = await _deleteValidator.ValidateAsync(id);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<object>(validation.Errors);

                var anuncio = await _AnuncioRepo.Obtener(a => a.AnuncioId == id);
                if (anuncio == null)
                    return ResponseHelper.Fail<object>("Anuncio no encontrado.", "Id", HttpStatusCode.NotFound);

                await _AnuncioRepo.Remover(anuncio);
                _logger.LogInformation("✅ Anuncio ID {Id} eliminado correctamente.", id);
                return ResponseHelper.Success<object>(null, "Anuncio eliminado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al eliminar anuncio ID {Id}", id);
                return ResponseHelper.FailException<object>(ex);
            }
        }

        public async Task<ApiResponse<AnuncioDto>> ActualizarAnuncioAsync(int id, AnuncioUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return ResponseHelper.Fail<AnuncioDto>("Datos inválidos para actualizar anuncio.", "Anuncio");

                var anuncioExistente = await _AnuncioRepo.Obtener(a => a.AnuncioId == id, tracked: true);
                if (anuncioExistente == null)
                    return ResponseHelper.Fail<AnuncioDto>("Anuncio no encontrado.", "Id", HttpStatusCode.NotFound);

                var validation = await _updateValidator.ValidateAsync(updateDto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<AnuncioDto>(validation.Errors);

                _mapper.Map(updateDto, anuncioExistente);
                await _AnuncioRepo.ActualizarAnuncio(anuncioExistente);

                _logger.LogInformation("✅ Anuncio ID {Id} actualizado correctamente.", id);
                return ResponseHelper.Success<AnuncioDto>(null, "Anuncio actualizado correctamente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al actualizar anuncio ID {Id}", id);
                return ResponseHelper.FailException<AnuncioDto>(ex);
            }
        }

        public async Task<ApiResponse<AnuncioDto>> ActualizarParcialAnuncioAsync(int id, JsonPatchDocument<AnuncioUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id <= 0)
                    return ResponseHelper.Fail<AnuncioDto>("Datos inválidos para la actualización parcial.", "Patch");

                var anuncioExistente = await _AnuncioRepo.Obtener(a => a.AnuncioId == id, tracked: true);
                if (anuncioExistente == null)
                    return ResponseHelper.Fail<AnuncioDto>("Anuncio no encontrado.", "Id", HttpStatusCode.NotFound);

                var dto = _mapper.Map<AnuncioUpdateDto>(anuncioExistente);
                patchDto.ApplyTo(dto);

                var validation = await _updateValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                    return ResponseHelper.Fail<AnuncioDto>(validation.Errors);

                _mapper.Map(dto, anuncioExistente);
                await _AnuncioRepo.ActualizarAnuncio(anuncioExistente);

                _logger.LogInformation("✅ PATCH aplicado correctamente al anuncio ID {Id}.", id);
                return ResponseHelper.Success<AnuncioDto>(null, "Anuncio actualizado parcialmente", HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al aplicar PATCH al anuncio ID {Id}", id);
                return ResponseHelper.FailException<AnuncioDto>(ex);
            }
        }




    }

}