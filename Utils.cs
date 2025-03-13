using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PersonnelHolidayPlanner;

public class Utils
{
    public static Dictionary<DateOnly, List<string>> generateTimeframe(
        int employeeId,
        DateOnly firstDay,
        DateOnly lastDay
    )
    {
        Dictionary<DateOnly, List<string>> result = new Dictionary<DateOnly, List<string>>();
        ICollection<Models.Project> projects = Program
            .context!.Employees.Find(employeeId)!
            .Projects;

        foreach (Models.Project project in projects)
        {
            int employeeCount = project.Employees.Count();

            for (DateOnly date = firstDay; date <= lastDay; date = date.AddDays(1))
            {
                int employeesOnLeave = project
                    .Employees.Where(e =>
                        e.Leaves.Any(l => l.StartDate <= date && l.EndDate >= date)
                    )
                    .Count();
                if (
                    employeesOnLeave == project.Employees.Count() - 1
                    && !hasLeaveOnDate(employeeId, date)
                )
                {
                    if (result.TryGetValue(date, out List<string>? projectList))
                    {
                        projectList.Add(project.Name);
                    }
                    else
                    {
                        result.Add(date, new List<string>() { project.Name });
                    }
                }
            }
        }

        return result;
    }

    public static Dictionary<DateOnly, string> getEmployeeLeaveDatesWithTypes(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return Program
            .context!.Leaves.Where(l =>
                l.EmployeeId == employeeId && l.StartDate <= endDate && l.EndDate >= startDate
            )
            .AsEnumerable()
            .SelectMany(l =>
                Enumerable
                    .Range(0, (l.EndDate.DayNumber - l.StartDate.DayNumber) + 1)
                    .Select(offset => l.StartDate.AddDays(offset))
                    .Where(d => d >= startDate && d <= endDate)
                    .Select(d => new { Date = d, l.LeaveType })
            )
            .ToDictionary(x => x.Date, x => x.LeaveType);
    }

    public static HashSet<DateOnly> getEmployeeLeaveDates(
        int employeeId,
        DateOnly startDate,
        DateOnly endDate
    )
    {
        return new HashSet<DateOnly>(
            Program
                .context!.Leaves.Where(l =>
                    l.EmployeeId == employeeId && l.StartDate <= endDate && l.EndDate >= startDate
                )
                .AsEnumerable()
                .SelectMany(l =>
                    Enumerable
                        .Range(0, (l.EndDate.DayNumber - l.StartDate.DayNumber) + 1)
                        .Select(offset => l.StartDate.AddDays(offset))
                        .Where(d => d >= startDate && d <= endDate)
                )
        );
    }

    public static void ExportLeavesToExcel(Dictionary<DateOnly, string> leaves, string filePath)
    {
        using (
            var spreadsheet = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook)
        )
        {
            // Create workbook parts
            WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Employee Leaves",
            };
            sheets.Append(sheet);

            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>()!;

            Row headerRow = new Row();
            headerRow.Append(
                new Cell() { DataType = CellValues.String, CellValue = new CellValue("Date") },
                new Cell() { DataType = CellValues.String, CellValue = new CellValue("Leave Type") }
            );
            sheetData.AppendChild(headerRow);

            // Add data rows
            foreach (KeyValuePair<DateOnly, string> entry in leaves.OrderBy(e => e.Key))
            {
                Row row = new();
                row.Append(
                    new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(entry.Key.ToString("yyyy-MM-dd")),
                    },
                    new Cell()
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue(entry.Value),
                    }
                );
                sheetData.AppendChild(row);
            }

            worksheetPart.Worksheet.Save();
            workbookPart.Workbook.Save();
        }
    }

    public static void ExportTimeframeToExcel(
        Dictionary<DateOnly, List<string>> timeframe,
        string filePath
    )
    {
        using SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(
            filePath,
            SpreadsheetDocumentType.Workbook
        );
        WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        SheetData sheetData = new SheetData();
        worksheetPart.Worksheet = new Worksheet(sheetData);

        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        sheets.Append(
            new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Project Conflicts",
            }
        );

        Row headerRow = new Row();
        headerRow.Append(
            new Cell { CellValue = new CellValue("Date"), DataType = CellValues.String },
            new Cell
            {
                CellValue = new CellValue("Conflict Projects"),
                DataType = CellValues.String,
            }
        );
        sheetData.Append(headerRow);

        foreach (KeyValuePair<DateOnly, List<string>> entry in timeframe.OrderBy(e => e.Key))
        {
            Row row = new();
            row.Append(
                new Cell
                {
                    CellValue = new CellValue(entry.Key.ToString("yyyy-MM-dd")),
                    DataType = CellValues.String,
                },
                new Cell
                {
                    CellValue = new CellValue(string.Join(", ", entry.Value)),
                    DataType = CellValues.String,
                }
            );
            sheetData.Append(row);
        }

        workbookPart.Workbook.Save();
    }

    public static bool hasLeaveOnDate(int employeeId, DateOnly date)
    {
        return Program.context!.Leaves.Any(leave =>
            leave.EmployeeId == employeeId && leave.StartDate <= date && leave.EndDate >= date
        );
    }
}
