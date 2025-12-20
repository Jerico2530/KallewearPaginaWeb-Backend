using ApiRopa.Repositorio;
using BiblotecaWeb.Datos;
using BiblotecaWeb;

namespace ApiRopa;
/*
     * PreguntaRepositorio
     *
     * Repositorio especializado para la administración de preguntas dentro del sistema.
     *
     * Funcionalidades clave:
     * - Actualización de preguntas existentes.
     * - Gestión centralizada de las operaciones de persistencia para preguntas.
     *
     * Propósito del componente:
     * Mantener de forma eficiente la información de preguntas en el sistema,
     * facilitando la edición y actualización de datos sin exponer la lógica de acceso a datos.
     *
     * Descripción del código:
     * - Implementa una operación específica de actualización sobre entidades Pregunta.
     * - Utiliza Entity Framework Core para persistir cambios en la base de datos.
     */
public class PreguntaRepositorio : Repositorio<Pregunta>, IPreguntaRepositorio
{
    private readonly AppDbContext _db;  // Contexto principal de persistencia de datos

    public PreguntaRepositorio(AppDbContext db) : base(db)
    {
        _db = db;
    }
    /// Actualiza una pregunta existente en la base de datos.
    public async Task<Pregunta> ActualizarPregunta(Pregunta entidad)
    {
        _db.Preguntas.Update(entidad);  // Marca la entidad como modificada para su persistencia
        await _db.SaveChangesAsync();  // Confirma los cambios en la base de datos
        return entidad;                // Devuelve el objeto actualizado
    }
}
