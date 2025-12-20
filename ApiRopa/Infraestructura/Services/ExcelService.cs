using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ApiRopa.Servicios
{
    public class ExcelService : IExcelService
    {
        public byte[] GenerarExcel<T>(
            IEnumerable<T> data,
            Dictionary<string, Func<T, object>> columnas,
            string titulo = "Reporte de Ventas",
            string nombreHoja = "Datos")
        {
            using (var package = new ExcelPackage())
            {
                var ws = package.Workbook.Worksheets.Add(nombreHoja);

                // ================================
                // 1) TÍTULO DEL REPORTE
                // ================================
                int filaTitulo = 1;
                ws.Cells[filaTitulo, 1].Value = titulo;
                ws.Cells[filaTitulo, 1, filaTitulo, columnas.Count].Merge = true;
                ws.Cells[filaTitulo, 1].Style.Font.Size = 16;
                ws.Cells[filaTitulo, 1].Style.Font.Bold = true;
                ws.Cells[filaTitulo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells[filaTitulo, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Row(filaTitulo).Height = 25;

                // ================================
                // 2) INFORMACIÓN CORPORATIVA
                // ================================
                int filaInfo = filaTitulo + 1;
                ws.Cells[filaInfo, 1].Value = "Generado por: Usuario del sistema";
                ws.Cells[filaInfo + 1, 1].Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy ");
                ws.Cells[filaInfo + 2, 1].Value = "Hora:  " + DateTime.Now.ToString("HH:mm:ss");
                ws.Cells[filaInfo, 1].Style.Font.Size = 10;
                ws.Cells[filaInfo + 1, 1].Style.Font.Size = 10;
                ws.Cells[filaInfo + 2, 1].Style.Font.Size = 10;

                // ================================
                // 3) ENCABEZADOS DE TABLA
                // ================================
                int filaEncabezados = filaInfo + 5;
                int col = 1;

                foreach (var header in columnas.Keys)
                {
                    var cell = ws.Cells[filaEncabezados, col];
                    cell.Value = header;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Color.SetColor(Color.White);
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(55, 71, 79));
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    col++;
                }
                ws.Row(filaEncabezados).Height = 20;

                // ================================
                // 3.1) AGREGAR FILTRO + CONGELAR FILA
                // ================================
                ws.Cells[filaEncabezados, 1, filaEncabezados, columnas.Count].AutoFilter = true;
                ws.View.FreezePanes(filaEncabezados + 1, 1);

                // ================================
                // 4) DATOS DE TABLA
                // ================================
                int filaDatos = filaEncabezados + 1;

                foreach (var item in data)
                {
                    int c = 1;
                    foreach (var colDef in columnas.Values)
                    {
                        var val = colDef(item);
                        var cell = ws.Cells[filaDatos, c];
                        cell.Value = val ?? "";
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Hair);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        if (val is string)
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        else
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        c++;
                    }
                    filaDatos++;
                }

                // ================================
                // 5) AJUSTAR COLUMNAS
                // ================================
                ws.Cells[1, 1, filaDatos, columnas.Count].AutoFitColumns(12, 50);

                // ================================
                // 6) BORDES PROFESIONALES
                // ================================
                using (var range = ws.Cells[filaEncabezados, 1, filaDatos - 1, columnas.Count])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                return package.GetAsByteArray();
            }
        }
    }
}
