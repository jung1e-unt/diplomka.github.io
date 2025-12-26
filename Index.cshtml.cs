
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

public class IndexModel : PageModel
{
    private readonly IConfiguration _config;

    public IndexModel(IConfiguration config)
    {
        _config = config;
    }

    // Разрешённые таблицы
    public static readonly HashSet<string> AllowedTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "lnpp.ANGA_OTK","lnpp.AUO_OPXR","lnpp.BMC_1_2","lnpp.DATACENTR","lnpp.DATACENTR_OLD","lnpp.EXCEL3","lnpp.GEST","lnpp.JA_1","lnpp.JA_2","lnpp.JASH_1",
    "lnpp.JASH_1_DUBL","lnpp.JASH_2","lnpp.JASH_2_DUBL","lnpp.LAB_LNPP","lnpp.LNGC_DATACENTR","lnpp.LNGC_OTK","lnpp.LNGC_OTK_DUBLE","lnpp.LPC3_MM",
    "lnpp.MEX_CGCA","lnpp.MEX_CGCA_LOG","lnpp.MEX_LAB_OB","lnpp.MEX_LPC1","lnpp.MEX_LPC1_DUBLE","lnpp.MEX_LPC1_LOG","lnpp.MEX_LPC2","lnpp.MEX_LPC2_LOG",
    "lnpp.MEX_LPC3","lnpp.MEX_LPC3_LOG","lnpp.MEX_LPC3_UCH","lnpp.MEX_LPC3_UCH_LOG","lnpp.MEX_SPC","lnpp.MEX_TRUBN","lnpp.MEX_TRUBN_LOG","lnpp.MICRO_CGCA",
    "lnpp.MICRO_LPC1","lnpp.MICRO_LPC1_NEW","lnpp.MICRO_LPC2","lnpp.MICRO_LPC3","lnpp.NDC_LNPP_DUBLE","lnpp.NDC_LNPP","lnpp.OTGR_CGCA","lnpp.OTGR_CGCA_NEW",
    "lnpp.OTGR_CGCA_OLD","lnpp.OTK","lnpp.PROD_LNPP","lnpp.PROD_LNPP_OLD","lnpp.PROIZ_LPC3","lnpp.PROIZV_ANGA","lnpp.PROIZV_ANGA_PRODUCTIV","lnpp.PROIZV_CGCA",
    "lnpp.PROIZV_LKPP_OLD","lnpp.PROIZV_LNGC","lnpp.PROIZV_LNGC_PRODUCTIV","lnpp.PROIZV_LNPP","lnpp.PROIZV_LNPP_OLD","lnpp.PROIZV_LNPP_PROD_NEW",
    "lnpp.PROIZV_LNPP_PRODUCTIV","lnpp.PROKAT_SMAZ","lnpp.PRPROKAT_CEXA","lnpp.RASTVOR_LNPP","lnpp.RASTVOR_OPXR2",
    "lnpp.XIM_VODA","lnpp.XIMBXU_LPC2","lnpp.XIMKISLOTA_LPC2","lnpp.XIMMASLO_LPC2","lnpp.XIMMUL_LPC2","lnpp.XIMNTA_LPC2","lnpp.XIMVXKONTR_LPC3",
    "lnpp.XIMXROM_LPC3"
    };

    private static readonly Dictionary<string, string> DefaultDateColumnByTable =
new(StringComparer.OrdinalIgnoreCase)
{
    ["lnpp.ANGA_OTK"] = "DATA",
    ["lnpp.AUO_OPXR"] = "DATA_ID",
    ["lnpp.BMC_1_2"] = "DATA_ID",
    ["lnpp.DATACENTR"] = "SAMPLE_DATETIME",
    ["lnpp.DATACENTR_OLD"] = "SAMPLE_DATETIME",
    ["lnpp.EXCEL3"] = "",
    ["lnpp.GEST"] = "DATA_ID",
    ["lnpp.JA_1"] = "SAMPLE_DATETIME",
    ["lnpp.JA_2"] = "SAMPLE_DATETIME",
    ["lnpp.JASH_1"] = "SAMPLE_DATETIME",
    ["lnpp.JASH_1_DUBL"] = "SAMPLE_DATETIME",
    ["lnpp.JASH_2"] = "SAMPLE_DATETIME",
    ["lnpp.JASH_2_DUBL"] = "SAMPLE_DATETIME",
    ["lnpp.LAB_LNPP"] = "DATA",
    ["lnpp.LNGC_DATACENTR"] = "SAMPLE_DATETIME",
    ["lnpp.LNGC_OTK"] = "DATA",
    ["lnpp.LNGC_OTK_DUBLE"] = "SAMPLE_DATETIME",
    ["lnpp.LPC3_MM"] = "DATA_PR",
    ["lnpp.MEX_CGCA"] = "DATA_ID",
    ["lnpp.MEX_CGCA_LOG"] = "DATA_ID",
    ["lnpp.MEX_LAB_OB"] = "DATA_ID_CG",
    ["lnpp.MEX_LPC1"] = "DATA_ID",
    ["lnpp.MEX_LPC1_DUBLE"] = "DATA_ID",
    ["lnpp.MEX_LPC1_LOG"] = "DATA_ID",
    ["lnpp.MEX_LPC2"] = "DATA_ID",
    ["lnpp.MEX_LPC2_LOG"] = "DATA_ID",
    ["lnpp.MEX_LPC3"] = "DATA_ID",
    ["lnpp.MEX_LPC3_LOG"] = "DATA_ID",
    ["lnpp.MEX_LPC3_UCH"] = "DATA_ID",
    ["lnpp.MEX_LPC3_UCH_LOG"] = "DATA_ID",
    ["lnpp.MEX_SPC"] = "DATA_ID",
    ["lnpp.MEX_TRUBN"] = "DATA_ID",
    ["lnpp.MEX_TRUBN_LOG"] = "DATA_ID",
    ["lnpp.MICRO_CGCA"] = "CREATED_DATE",
    ["lnpp.MICRO_LPC1"] = "CREATED_DATE",
    ["lnpp.MICRO_LPC2"] = "CREATED_DATE",
    ["lnpp.MICRO_LPC3"] = "CREATED_DATE",
    ["lnpp.NDC_LNPP_DUBLE"] = "SAMPLE_DATETIME",
    ["lnpp.NDC_LNPP"] = "SAMPLE_DATETIME",
    ["lnpp.OTGR_CGCA"] = "DATA_CEHA",
    ["lnpp.OTGR_CGCA_NEW"] = "DATA_CEHA",
    ["lnpp.OTGR_CGCA_OLD"] = "DATA_CEHA",
    ["lnpp.OTK"] = "DATA",
    ["lnpp.PROD_LNPP"] = "DATA",
    ["lnpp.PROD_LNPP_OLD"] = "DATA",
    ["lnpp.PROIZ_LPC3"] = "DATA",
    ["lnpp.PROIZV_ANGA"] = "DATA",
    ["lnpp.PROIZV_ANGA_PRODUCTIV"] = "DATA",
    ["lnpp.PROIZV_CGCA"] = "DATA",
    ["lnpp.PROIZV_LKPP_OLD"] = "DATA",
    ["lnpp.PROIZV_LNGC"] = "DATA",
    ["lnpp.PROIZV_LNGC_PRODUCTIV"] = "DATA",
    ["lnpp.PROIZV_LNPP"] = "DATA",
    ["lnpp.PROIZV_LNPP_OLD"] = "DATA",
    ["lnpp.PROIZV_LNPP_PROD_NEW"] = "DATA",
    ["lnpp.PROIZV_LNPP_PRODUCTIV"] = "DATA",
    ["lnpp.PROKAT_SMAZ"] = "DATA_ID",
    ["lnpp.PRPROKAT_CEXA"] = "DATA_ID",
    ["lnpp.RASTVOR_LNPP"] = "DATA",
    ["lnpp.RASTVOR_OPXR2"] = "DATA_ID",
    ["lnpp.XIM_VODA"] = "DATA_ID",
    ["lnpp.XIMBXU_LPC2"] = "DATA_ID",
    ["lnpp.XIMKISLOTA_LPC2"] = "DATA_ID",
    ["lnpp.XIMMASLO_LPC2"] = "DATA_ID",
    ["lnpp.XIMMUL_LPC2"] = "DATA_ID",
    ["lnpp.XIMNTA_LPC2"] = "DATA_ID",
    ["lnpp.XIMVXKONTR_LPC3"] = "DATA_ID",
    ["lnpp.XIMXROM_LPC3"] = "DATA_ID"
};


    private static readonly HashSet<string> DefaultExcludeColumns =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "ORA_ROWSCN","SYS_CREATION_DATE","SYS_UPDATE_DATE"
        };

    private const string ContentTypeXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";



    public IActionResult OnGetData(
        string table,
        string? startDate,
        string? endDate,
        string? nPart,
        string? nPlaw,
        int? offset,
        int? pageSize
    )
    {
        if (string.IsNullOrWhiteSpace(table))
            return new JsonResult(new { error = "Table is required." }) { StatusCode = 400 };

        var tableUpper = table.Trim().ToUpperInvariant();
        if (!AllowedTables.Contains(tableUpper))
            return new JsonResult(new { error = "Table is not allowed." }) { StatusCode = 400 };

        var dateColumn = ResolveDateColumn(tableUpper);
        var start = ParseDate(startDate);
        var end = ParseDate(endDate);

        var take = Math.Clamp(pageSize ?? 100, 1, 1000);
        var skip = Math.Max(offset ?? 0, 0);

        var connString = _config.GetConnectionString("OracleConnection");
        using var conn = new OracleConnection(connString);
        conn.Open();

        try
        {
            var columns = GetColumns(conn, tableUpper, DefaultExcludeColumns);

            if (!columns.Any(c => string.Equals(c, dateColumn, StringComparison.OrdinalIgnoreCase)))
                return new JsonResult(new { error = $"Колонка даты '{dateColumn}' отсутствует в {tableUpper}." }) { StatusCode = 500 };

            var orderBy = dateColumn;
            if (string.IsNullOrWhiteSpace(orderBy) || !columns.Contains(orderBy, StringComparer.OrdinalIgnoreCase))
                orderBy = columns[0];

            var sql = BuildSqlPaged(tableUpper, columns, dateColumn, start, end, nPart, nPlaw, orderBy, skip, take);
            using var cmd = new OracleCommand(sql, conn);
            BindDateParams(cmd, start, end);

            if (!string.IsNullOrWhiteSpace(nPart) &&
                columns.Any(c => string.Equals(c, "N_PART", StringComparison.OrdinalIgnoreCase)))
            {
                cmd.Parameters.Add(new OracleParameter("pNPart", nPart));
            }

            if (!string.IsNullOrWhiteSpace(nPlaw) &&
                columns.Any(c => string.Equals(c, "N_PLAW", StringComparison.OrdinalIgnoreCase)))
            {
                cmd.Parameters.Add(new OracleParameter("pNPlaw", nPlaw));
            }

            var rows = new List<Dictionary<string, object?>>(capacity: take);
            using var reader = cmd.ExecuteReader();
            var schema = reader.GetColumnSchema();

            while (reader.Read())
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

                foreach (var col in schema)
                {
                    var raw = reader[col.ColumnName];

                    if (raw == DBNull.Value)
                    {
                        row[col.ColumnName] = null;
                    }
                    else if (raw is DateTime dt)
                    {

                        if (TimeColumns2.Contains(col.ColumnName))
                        {
                            row[col.ColumnName] = dt.ToString("hh:mm");
                        }
                        else if (TimeColumns.Contains(col.ColumnName))
                        {
                            row[col.ColumnName] = dt.ToString("dd.MM.yyyy hh:mm:ss");
                        }
                        else
                        {
                            row[col.ColumnName] = dt.ToString("dd.MM.yyyy");
                        }
                    }
                    else
                    {
                        row[col.ColumnName] = raw;
                    }
                }

                rows.Add(row);
            }

            bool hasMore = false;
            if (rows.Count > take)
            {
                hasMore = true;
                rows.RemoveAt(rows.Count - 1);
            }

            return new JsonResult(new
            {
                data = rows,
                nextOffset = skip + rows.Count,
                hasMore
            }, new JsonSerializerOptions { PropertyNamingPolicy = null, WriteIndented = false });
        }
        catch (OracleException ex) when (ex.Number == 8180 || ex.Number == 8181)
        {
            return new JsonResult(new { data = Array.Empty<object>(), nextOffset = skip, hasMore = false });
        }
    }


    private static string BuildSqlPaged(
    string tableUpper,
    IReadOnlyList<string> selectedColumns,
    string dateColumn,
    DateTime? startDate,
    DateTime? endDate,
    string? nPart,
    string? nPlaw,
    string orderBy,
    int offset,
    int pageSize)
    {
        if (selectedColumns is null || selectedColumns.Count == 0)
            throw new ArgumentException("Нужно указать хотя бы одну колонку для SELECT.", nameof(selectedColumns));

        var selectList = string.Join(", ", selectedColumns.Select(c => $"\"{c}\""));

        var filters = new List<string>();

        if (startDate.HasValue && endDate.HasValue)
            filters.Add($"\"{dateColumn}\" BETWEEN :pStart AND :pEnd");
        else if (startDate.HasValue)
            filters.Add($"\"{dateColumn}\" >= :pStart");
        else if (endDate.HasValue)
            filters.Add($"\"{dateColumn}\" <= :pEnd");
        else
            filters.Add($"\"{dateColumn}\" BETWEEN ADD_MONTHS(SYSDATE, -6) AND SYSDATE");

        if (!string.IsNullOrWhiteSpace(nPart) && selectedColumns.Any(c => string.Equals(c, "N_PART", StringComparison.OrdinalIgnoreCase)))
            filters.Add("\"N_PART\" = :pNPart");

        if (!string.IsNullOrWhiteSpace(nPlaw) && selectedColumns.Any(c => string.Equals(c, "N_PLAW", StringComparison.OrdinalIgnoreCase)))
            filters.Add("\"N_PLAW\" = :pNPlaw");

        var where = filters.Count > 0 ? $"WHERE {string.Join(" AND ", filters)}" : string.Empty;

        var order = $"ORDER BY \"{orderBy}\" DESC";

        var fetch = $"OFFSET {offset} ROWS FETCH NEXT {pageSize + 1} ROWS ONLY";

        var sql = $@"
SELECT {selectList}
FROM {tableUpper}
{where}
{order}
{fetch}";
        return sql;
    }



    public IActionResult OnGetExport(string table, string? startDate, string? endDate, string? nPart, string? nPlaw)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(table))
                return BadRequest("Table is required.");

            var tableUpper = table.Trim().ToUpperInvariant();
            if (!AllowedTables.Contains(tableUpper))
                return BadRequest("Table is not allowed.");

            var dateColumn = ResolveDateColumn(tableUpper);
            var start = ParseDate(startDate);
            var end = ParseDate(endDate);

            var connString = _config.GetConnectionString("OracleConnection");
            using var conn = new OracleConnection(connString);
            conn.Open();

            var columns = GetColumns(conn, tableUpper, DefaultExcludeColumns);
            if (!columns.Any(c => string.Equals(c, dateColumn, StringComparison.OrdinalIgnoreCase)))
                return BadRequest($"Колонка даты '{dateColumn}' отсутствует в {tableUpper}.");

            var sql = BuildSql(tableUpper, columns, dateColumn, start, end, nPart, nPlaw);

            using var cmd = new OracleCommand(sql, conn)
            {
                BindByName = true
            };

            BindDateParams(cmd, start, end);

            if (!string.IsNullOrWhiteSpace(nPart) &&
                columns.Any(c => string.Equals(c, "N_PART", StringComparison.OrdinalIgnoreCase)))
            {
                cmd.Parameters.Add(new OracleParameter("pNPart", OracleDbType.Varchar2) { Value = nPart });
            }

            if (!string.IsNullOrWhiteSpace(nPlaw) &&
                columns.Any(c => string.Equals(c, "N_PLAW", StringComparison.OrdinalIgnoreCase)))
            {
                cmd.Parameters.Add(new OracleParameter("pNPlaw", OracleDbType.Varchar2) { Value = nPlaw });
            }

            using var reader = cmd.ExecuteReader();
            var dt = ToDataTable(reader);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(dt, tableUpper);

            var tbl = ws.Tables.FirstOrDefault();
            if (tbl != null) tbl.ShowAutoFilter = true;

            ws.SheetView.FreezeRows(1);
            ApplyDateColumnFormats(ws, dt);
            ApplyDateColumnFormats2(ws, dt);
            ws.Columns().AdjustToContents();

            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                wb.SaveAs(ms);
                bytes = ms.ToArray();
            }

            var fileName = $"{tableUpper}_{DateTime.Now:ddMMyy}.xlsx";
            return File(bytes, ContentTypeXlsx, fileName);
        }
        catch (OracleException ex) when (ex.Number == 8180 || ex.Number == 8181)
        {
            using var wbEmpty = new XLWorkbook();
            var wsEmpty = wbEmpty.Worksheets.Add(table.ToUpperInvariant());
            wsEmpty.Cell(1, 1).Value = "Нет данных";
            wsEmpty.Columns().AdjustToContents();

            byte[] emptyBytes;
            using (var msEmpty = new MemoryStream())
            {
                wbEmpty.SaveAs(msEmpty);
                emptyBytes = msEmpty.ToArray();
            }

            return File(emptyBytes, ContentTypeXlsx, $"{table.ToUpperInvariant()}_{DateTime.Now:ddMMyy}.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Export failed: {ex.GetType().Name}: {ex.Message}");
        }
    }



    private static string ResolveDateColumn(string tableUpper)
        => DefaultDateColumnByTable.TryGetValue(tableUpper, out var col) ? col : "DT";




    private static string BuildSql(
        string tableUpper,
        IReadOnlyList<string> selectedColumns,
        string dateColumn,
        DateTime? startDate,
        DateTime? endDate,
        string? nPart,
        string? nPlaw)
    {
        if (selectedColumns is null || selectedColumns.Count == 0)
            throw new ArgumentException("Нужно указать хотя бы одну колонку для SELECT.", nameof(selectedColumns));

        if (string.IsNullOrWhiteSpace(tableUpper))
            throw new ArgumentException("Имя таблицы не может быть пустым.", nameof(tableUpper));

        if (string.IsNullOrWhiteSpace(dateColumn))
            throw new ArgumentException("Имя колонки даты не может быть пустым.", nameof(dateColumn));

        var selectList = string.Join(", ", selectedColumns.Select(c => $"\"{c}\""));
        var parts = new List<string>();

        if (startDate.HasValue && endDate.HasValue)
        {
            parts.Add($"\"{dateColumn}\" BETWEEN :pStart AND :pEnd");
        }
        else if (startDate.HasValue)
        {
            parts.Add($"\"{dateColumn}\" >= :pStart");
        }
        else if (endDate.HasValue)
        {
            parts.Add($"\"{dateColumn}\" <= :pEnd");
        }
        else
        {
            parts.Add($"\"{dateColumn}\" BETWEEN ADD_MONTHS(SYSDATE, -6) AND SYSDATE");
        }

        if (!string.IsNullOrWhiteSpace(nPart) &&
            selectedColumns.Any(c => string.Equals(c, "N_PART", StringComparison.OrdinalIgnoreCase)))
        {
            parts.Add("\"N_PART\" = :pNPart");
        }

        if (!string.IsNullOrWhiteSpace(nPlaw) &&
            selectedColumns.Any(c => string.Equals(c, "N_PLAW", StringComparison.OrdinalIgnoreCase)))
        {
            parts.Add("\"N_PLAW\" = :pNPlaw");
        }

        var where = parts.Count > 0 ? $"WHERE {string.Join(" AND ", parts)}" : string.Empty;

        var sql = $@"
SELECT {selectList}
FROM {tableUpper}
{where}";

        return sql;
    }







    private static void BindDateParams(OracleCommand cmd, DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue)
            cmd.Parameters.Add(new OracleParameter("pStart", OracleDbType.Date) { Value = startDate.Value.Date });

        if (endDate.HasValue)
        {
            var inclusiveEnd = endDate.Value.Date.AddDays(1).AddSeconds(-1);
            cmd.Parameters.Add(new OracleParameter("pEnd", OracleDbType.Date) { Value = inclusiveEnd });
        }
    }






    public IActionResult OnGetTablesState()
    {
        var connString = _config.GetConnectionString("OracleConnection");
        using var conn = new OracleConnection(connString);
        conn.Open();

        var list = new List<object>();
        foreach (var table in AllowedTables)
        {
            try
            {
                var cols = GetColumns(conn, table, DefaultExcludeColumns);
                var hasPart = cols.Any(c => string.Equals(c, "N_PART", StringComparison.OrdinalIgnoreCase));
                var hasPlaw = cols.Any(c => string.Equals(c, "N_PLAW", StringComparison.OrdinalIgnoreCase));
                bool hasData = false;
                using (var cmd = new OracleCommand($"SELECT 1 FROM {table} FETCH FIRST 1 ROWS ONLY", conn))
                using (var r = cmd.ExecuteReader())
                    hasData = r.Read();

                list.Add(new { table, hasData, hasPart, hasPlaw });
            }
            catch
            {
                list.Add(new { table, hasData = false, hasPart = false, hasPlaw = false });
            }
        }
        return new JsonResult(list, new JsonSerializerOptions { PropertyNamingPolicy = null });
    }



    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        var formats = new[] { "yyyy-MM-dd", "dd.MM.yyyy", "dd.MM.yy" };
        if (DateTime.TryParseExact(s.Trim(), formats, CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeLocal, out var dt))
            return dt.Date;

        if (DateTime.TryParse(s, out dt)) return dt.Date;
        return null;
    }

    private static List<string> GetColumns(OracleConnection conn, string tableUpper, HashSet<string> exclude)
    {
        var sql = $"SELECT * FROM {tableUpper} FETCH FIRST 1 ROWS ONLY";
        using var cmd = new OracleCommand(sql, conn);
        using var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);

        var schemaTable = reader.GetSchemaTable();
        var cols = new List<string>();

        if (schemaTable != null)
        {
            foreach (DataRow row in schemaTable.Rows)
            {
                var name = row["ColumnName"]?.ToString();
                if (!string.IsNullOrWhiteSpace(name) && !exclude.Contains(name))
                    cols.Add(name);
            }
        }

        if (cols.Count == 0)
            throw new InvalidOperationException($"Не удалось получить колонки таблицы {tableUpper}.");

        return cols;
    }


    private static readonly HashSet<string> TimeColumns = new(StringComparer.OrdinalIgnoreCase)
    {
    "created_date", "updated_date", "deleted_date","create_date","sample_datetime","date_create",

    };

    private static readonly HashSet<string> TimeColumns2 = new(StringComparer.OrdinalIgnoreCase)
    {
    "TIMES_NACH","TIMES_KON","T1","VREMYA"
    };

    private static DataTable ToDataTable(OracleDataReader reader)
    {
        var dt = new DataTable();
        var schema = reader.GetColumnSchema();

        foreach (var col in schema)
        {
            var type = col.DataType ?? typeof(string);
            if (type == typeof(object) && !string.IsNullOrEmpty(col.DataTypeName))
            {
                var name = col.DataTypeName.ToUpperInvariant();
                if (name.Contains("DATE") || name.Contains("TIMESTAMP"))
                    type = typeof(DateTime);
                else if (name.Contains("NUMBER"))
                    type = typeof(decimal);
                else if (name.Contains("FLOAT") || name.Contains("BINARY_FLOAT") || name.Contains("BINARY_DOUBLE"))
                    type = typeof(double);
            }

            dt.Columns.Add(col.ColumnName, type);
        }

        while (reader.Read())
        {
            var row = dt.NewRow();
            for (int i = 0; i < dt.Columns.Count; i++)
                row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

            dt.Rows.Add(row);
        }

        return dt;
    }



    //public IActionResult OnGetRunSqlPaged(string sql, int page = 1, int pageSize = 100)
    //{
    //    if (string.IsNullOrWhiteSpace(sql))
    //        return new JsonResult(new { error = "SQL-запрос обязателен." }) { StatusCode = 400 };

    //    if (!sql.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
    //        return new JsonResult(new { error = "Разрешены только SELECT-запросы." }) { StatusCode = 400 };

    //    var selectedTable = HttpContext.Request.Query["currentTable"].ToString();
    //    if (string.IsNullOrWhiteSpace(selectedTable))
    //        return new JsonResult(new { error = "Сначала выберите таблицу слева." }) { StatusCode = 400 };

    //    var tablesInSql = ExtractTablesFromSql(sql);
    //    if (tablesInSql.Any(t => !string.Equals(t, selectedTable, StringComparison.OrdinalIgnoreCase)))
    //        return new JsonResult(new { error = "Запрос содержит таблицы, которые не выбраны." }) { StatusCode = 400 };

    //    if (!AllowedTables.Contains(selectedTable.ToUpperInvariant()))
    //        return new JsonResult(new { error = "Таблица не разрешена." }) { StatusCode = 400 };

    //    if (page < 1) page = 1;
    //    if (pageSize < 1) pageSize = 100;
    //    if (pageSize > 1000) pageSize = 1000; 

    //    if (sql.TrimEnd().EndsWith(";"))
    //        sql = sql.TrimEnd().TrimEnd(';');

    //    var offset = (page - 1) * pageSize;

    //    var connString = _config.GetConnectionString("OracleConnection");
    //    using var conn = new OracleConnection(connString);
    //    conn.Open();

    //    var pagedSql = $@"
    //    SELECT * FROM (
    //        {sql}
    //    ) t
    //    OFFSET :offset ROWS FETCH NEXT :pageSize ROWS ONLY";

    //    using var cmd = new OracleCommand(pagedSql, conn);
    //    cmd.BindByName = true;
    //    cmd.Parameters.Add(new OracleParameter("offset", OracleDbType.Int32) { Value = offset });
    //    cmd.Parameters.Add(new OracleParameter("pageSize", OracleDbType.Int32) { Value = pageSize });

    //    using var reader = cmd.ExecuteReader();

    //    var rows = new List<Dictionary<string, object?>>();
    //    var schema = reader.GetColumnSchema();
    //    while (reader.Read())
    //    {
    //        var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
    //        foreach (var col in schema)
    //        {
    //            var raw = reader[col.ColumnName];
    //            if (raw == DBNull.Value)
    //                row[col.ColumnName] = null;
    //            else if (raw is DateTime dt)
    //                row[col.ColumnName] = dt.ToString("dd.MM.yyyy");
    //            else
    //                row[col.ColumnName] = raw;
    //        }
    //        rows.Add(row);
    //    }

    //    bool hasMore;
    //    using (var cmd2 = new OracleCommand(@$"
    //    SELECT COUNT(*) FROM (
    //        SELECT 1 FROM (
    //            {sql}
    //        ) t
    //        OFFSET :offset2 ROWS FETCH NEXT 1 ROWS ONLY
    //    )", conn))
    //    {
    //        cmd2.BindByName = true;
    //        cmd2.Parameters.Add(new OracleParameter("offset2", OracleDbType.Int32) { Value = offset + pageSize });
    //        var countObj = cmd2.ExecuteScalar();
    //        var cnt = Convert.ToInt32(countObj);
    //        hasMore = cnt > 0;
    //    }

    //    var columns = schema.Select(c => c.ColumnName).ToArray();

    //    return new JsonResult(new
    //    {
    //        columns,
    //        rows,
    //        page,
    //        pageSize,
    //        hasMore
    //    });
    //} 


    //private static List<string> ExtractTablesFromSql(string sql)
    //{
    //    var result = new List<string>();
    //    var tokens = sql.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    //    for (int i = 0; i < tokens.Length; i++)
    //    {
    //        if (tokens[i].Equals("FROM", StringComparison.OrdinalIgnoreCase) && i + 1 < tokens.Length)
    //        {
    //            result.Add(tokens[i + 1].Trim(',', ';'));
    //        }
    //        if (tokens[i].Equals("JOIN", StringComparison.OrdinalIgnoreCase) && i + 1 < tokens.Length)
    //        {
    //            result.Add(tokens[i + 1].Trim(',', ';'));
    //        }
    //    }
    //    return result;
    //}






    private static void ApplyDateColumnFormats(IXLWorksheet ws, DataTable dt)
    {
        var timeColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "created_date", "updated_date", "deleted_date",
        "create_date", "sample_datetime", "date_create"
    };

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            var col = dt.Columns[i];

            if (col.DataType != typeof(DateTime))
                continue;

            if (timeColumns.Contains(col.ColumnName))
            {
                ws.Column(i + 1).Style.NumberFormat.Format = "dd.MM.yyyy HH:mm";
            }
            else
            {
                ws.Column(i + 1).Style.NumberFormat.Format = "dd.MM.yyyy";
            }
        }
    }

    private static void ApplyDateColumnFormats2(IXLWorksheet ws, DataTable dt)
    {
        var timeColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "TIMES_NACH", "TIMES_KON", "T1", "VREMYA"
    };

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            var col = dt.Columns[i];

            if (col.DataType != typeof(DateTime))
                continue;

            if (!timeColumns.Contains(col.ColumnName))
                continue;

            var xlCol = ws.Column(i + 1);

            foreach (var cell in xlCol.CellsUsed().Skip(1)) // без заголовка
            {
                if (cell.TryGetValue<DateTime>(out var dtVal))
                {
                    // КЛЮЧЕВО: DateTime → TimeSpan
                    cell.Value = dtVal.TimeOfDay;
                }
            }

            xlCol.Style.NumberFormat.Format = "hh:mm";
        }
    }

}
