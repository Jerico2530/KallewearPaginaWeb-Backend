namespace ApiRopa;

public interface IExcelService
{
    byte[] GenerarExcel<T>(
        IEnumerable<T> data,
        Dictionary<string, Func<T, object>> columnas,
        string titulo = "Reporte",
        string nombreHoja = "Datos");
}
