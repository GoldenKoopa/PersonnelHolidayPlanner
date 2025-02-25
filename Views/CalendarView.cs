using Spectre.Console;

namespace PersonnelHolidayPlanner.Views;

public class CalendarView : View
{
    static List<DateTime> leaveDays = new()
    {
        DateTime.Now.AddDays(2).Date,
        DateTime.Now.AddDays(3).Date,
        DateTime.Now.AddDays(5).Date,
        DateTime.Now.AddDays(-2).Date,
        DateTime.Now.AddDays(-5).Date,
        DateTime.Now.AddDays(-7).Date,
    };

    DateTime currentDate = DateTime.Now;

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

            var currentDay = new DateTime(currentDate.Year, currentDate.Month, day);
            if (day == currentDate.Day)
            {
                currentRow.Add($"[bold yellow]{day}[/]"); // Highlight the selected day
            }
            else if (leaveDays.Contains(currentDay))
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

        AnsiConsole.Write(
            new Panel($"[bold green]You selected: {currentDate.ToShortDateString()}[/]")
                .Header("test header", Justify.Center)
                .BorderColor(Color.Red)
        );
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
}
