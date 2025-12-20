using ApiRopa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
/*
 * ExcelGenericoService
 *
 * Servicio encargado de la generación de reportes Excel genéricos a partir de colecciones de datos.
 * Funcionalidades clave:
 * - Exportar listas de objetos a un archivo Excel.
 * - Respetar atributos de visualización (DisplayAttribute) para los nombres de columna.
 * - Formatear valores especiales como booleanos y fechas.
 * - Excluir propiedades específicas si se requiere.
 *
 * Propósito del componente:
 * Centralizar la lógica de exportación de datos a Excel, asegurando consistencia y flexibilidad
 * para diferentes tipos de entidades, desacoplando la generación de Excel de la lógica de negocio.
 *
 * Este servicio actúa como capa de infraestructura independiente, facilitando pruebas unitarias,
 * reutilización de código y manteniendo el código limpio y profesional.
 */
public class ExcelGenericoService
{

    private readonly IExcelService _excelService;
    // Inyección del servicio especializado en generación de archivos Excel
    public ExcelGenericoService(IExcelService excelService)
    {
        _excelService = excelService;
    }
    /// <summary>
    /// Exporta una colección de datos a un archivo Excel.
    /// </summary>
    /// <typeparam name="T">Tipo de los objetos a exportar</typeparam>
    /// <param name="data">Colección de datos</param>
    /// <param name="titulo">Título del reporte</param>
    /// <param name="nombreHoja">Nombre de la hoja Excel</param>
    /// <param name="propiedadesExcluir">Lista de propiedades a excluir del Excel</param>
    public async Task<byte[]> ExportarExcel<T>(
        IEnumerable<T> data,
        string titulo = "Reporte",
        string nombreHoja = "Datos",
        string[] propiedadesExcluir = null) // propiedades a ignorar
    {
        // Obtener todas las propiedades públicas de T, excluyendo las indicadas
        var propiedades = typeof(T).GetProperties()
            .Where(p => propiedadesExcluir == null || !propiedadesExcluir.Contains(p.Name));

        // Crear diccionario: nombre de columna -> función que obtiene el valor del objeto
        var columnas = propiedades.ToDictionary(
            prop =>
            {
                // Revisar si tiene DisplayAttribute
                var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
                return displayAttr != null ? displayAttr.Name : prop.Name;
            },
            prop => (Func<T, object>)(item =>
            {
                // Obtener valor de la propiedad y aplicar formatos especiales
                var val = prop.GetValue(item);
                if (val is bool b) return b ? "Activo" : "Inactivo";
                if (val is DateTime dt) return dt.ToString("yyyy-MM-dd");
                return val ?? "";
            })
        );
        // Delegar generación del Excel al servicio especializado
        return _excelService.GenerarExcel(data, columnas, titulo, nombreHoja);
    }
}
