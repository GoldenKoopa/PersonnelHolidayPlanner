using System.Globalization;
using DotNetEnv;
using Spectre.Console;

namespace PersonnelHolidayPlanner.Views;

public class CalendarView : View
{
    private int employeeId;
    DateTime currentDate = DateTime.Now;
    DBContext.PHPContext context = Program.context;

    private Dictionary<DateOnly, string> leaveTypes;
    private Dictionary<string, Dictionary<DateOnly, List<string>>> projectDays;

    public CalendarView()
    {
        Env.Load();
        // if
        // this.employeeId = Environment.GetEnvironmentVariable("employee_id");
        this.employeeId = 1;
        leaveTypes = new Dictionary<DateOnly, string>();
        foreach (Models.Leave leave in context.Employees.Find(employeeId)!.Leaves)
        {
            DateOnly startDate = leave.StartDate;
            DateOnly endDate = leave.EndDate;
            string type = leave.LeaveType;

            DateOnly date = startDate;
            while (date <= endDate)
            {
                leaveTypes[date] = type;
                date = date.AddDays(1);
            }
        }
        projectDays = new Dictionary<string, Dictionary<DateOnly, List<string>>>();
        getOrCreateMonth(currentDate);
    }

    public void renderView()
    {
        Table table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold blue]Sun[/]")
            .AddColumn("[bold blue]Mon[/]")
            .AddColumn("[bold blue]Tue[/]")
            .AddColumn("[bold blue]Wed[/]")
            .AddColumn("[bold blue]Thu[/]")
            .AddColumn("[bold blue]Fri[/]")
            .AddColumn("[bold blue]Sat[/]");

        DateTime firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        Dictionary<DateOnly, List<string>> projectDaysMonth = getOrCreateMonth(currentDate);

        List<string> currentRow = new();
        for (int i = 0; i < (int)firstDayOfMonth.DayOfWeek; i++)
        {
            currentRow.Add(""); // Empty cells before the first day
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            if (currentRow.Count == 7)
            {
                table.AddRow(currentRow.ToArray());
                currentRow.Clear();
            }

            DateTime currentDayDateTime = new DateTime(currentDate.Year, currentDate.Month, day);
            DateOnly currentDay = DateOnly.FromDateTime(currentDayDateTime);
            if (day == currentDate.Day)
            {
                currentRow.Add($"[bold yellow]{day}[/]"); // Highlight the selected day
            }
            else if (leaveTypes.ContainsKey(currentDay))
            {
                currentRow.Add($"[bold green]{day}[/]");
            }
            else if (projectDaysMonth.ContainsKey(currentDay))
            {
                currentRow.Add($"[bold red]{day}[/]");
            }
            else
            {
                currentRow.Add(day.ToString());
            }
        }

        if (currentRow.Count > 0)
        {
            while (currentRow.Count < 7)
                currentRow.Add(""); // Fill remaining cells in the last row

            table.AddRow(currentRow.ToArray());
        }

        AnsiConsole.Write(
            new Panel(table)
                .Header($"[bold green]{currentDate:MMMM yyyy}[/]", Justify.Center)
                .BorderColor(Color.Blue)
        );

        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        string text = $"[bold green]You selected: {currentDate.ToShortDateString()}[/]";
        DateOnly currentDayOnly = DateOnly.FromDateTime(currentDate);
        if (leaveTypes.ContainsKey(currentDayOnly))
        {
            if (leaveTypes.TryGetValue(currentDayOnly, out string? type))
            {
                text += $"\n[bold]Leave Type:[/] {type}";
            }
        }
        else if (projectDaysMonth.TryGetValue(currentDayOnly, out List<string>? projects))
        {
            text += $"\n[bold]Blocked by following projects:[/]";
            foreach (string project in projects)
            {
                text += $"\n[bold]   - {project}[/]";
            }
        }
        AnsiConsole.Write(new Panel(text).Header("Info", Justify.Center).BorderColor(Color.Red));
    }

    public void handleKeys(ConsoleKeyInfo keyInfo)
    {
        ConsoleKey key = keyInfo.Key;
        ConsoleModifiers keyModifiers = keyInfo.Modifiers;
        switch (key)
        {
            // navigation
            case ConsoleKey.LeftArrow:
                currentDate = currentDate.AddDays(-1);
                break;
            case ConsoleKey.RightArrow:
                currentDate = currentDate.AddDays(1);
                break;
            case ConsoleKey.UpArrow:
                if (keyModifiers.HasFlag(ConsoleModifiers.Shift))
                {
                    currentDate = currentDate.AddMonths(-1);
                    break;
                }
                currentDate = currentDate.AddDays(-7);
                break;
            case ConsoleKey.DownArrow:
                if (keyModifiers.HasFlag(ConsoleModifiers.Shift))
                {
                    currentDate = currentDate.AddMonths(1);
                    break;
                }
                currentDate = currentDate.AddDays(7);
                break;

            // navigation vim-like
            case ConsoleKey.H:
                currentDate = currentDate.AddDays(-1);
                break;
            case ConsoleKey.L:
                currentDate = currentDate.AddDays(1);
                break;
            case ConsoleKey.K:
                currentDate = currentDate.AddDays(-7);
                break;
            case ConsoleKey.J:
                currentDate = currentDate.AddDays(7);
                break;
            case ConsoleKey.N:
                currentDate = currentDate.AddMonths(1);
                break;
            case ConsoleKey.P:
                currentDate = currentDate.AddMonths(-1);
                break;

            // selection
            case ConsoleKey.Enter:
                AnsiConsole.MarkupLine(
                    $"[bold green]You selected: {currentDate.ToShortDateString()}[/]"
                );
                Environment.Exit(0);
                break;

            // exit
            case ConsoleKey.Escape:
                Environment.Exit(0);
                break;
            case ConsoleKey.Q:
                Environment.Exit(0);
                break;
        }
    }

    public Dictionary<DateOnly, List<string>> getOrCreateMonth(DateTime currentDate)
    {
        Dictionary<DateOnly, List<string>>? month;
        if (
            !projectDays.TryGetValue(
                currentDate.Month.ToString() + currentDate.Year.ToString(),
                out month
            )
        )
        {
            DateOnly firstDayOfMonth = DateOnly.FromDateTime(
                new DateTime(currentDate.Year, currentDate.Month, 1)
            );
            DateOnly lastDayOfMonth = DateOnly.FromDateTime(
                new DateTime(
                    currentDate.Year,
                    currentDate.Month,
                    DateTime.DaysInMonth(currentDate.Year, currentDate.Month)
                )
            );
            Dictionary<DateOnly, List<string>> result = Utils.generateTimeframe(
                employeeId,
                firstDayOfMonth,
                lastDayOfMonth
            );
            projectDays.Add(currentDate.Month.ToString() + currentDate.Year.ToString(), result);
            return result;
        }
        return month;
    }
}
