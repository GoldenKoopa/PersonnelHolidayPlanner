// using System;
using Spectre.Console;

class Program
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

    static void Main(string[] args)
    {
        DateTime currentDate = DateTime.Now;

        while (true)
        {
            Console.Clear();
            DisplayCalendar(currentDate);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            ConsoleKey key = keyInfo.Key;
            ConsoleModifiers modifiers = keyInfo.Modifiers;
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
                    if (modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        currentDate = currentDate.AddMonths(-1);
                        break;
                    }
                    currentDate = currentDate.AddDays(-7);
                    break;
                case ConsoleKey.DownArrow:
                    if (modifiers.HasFlag(ConsoleModifiers.Shift))
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
                    return;

                // exit
                case ConsoleKey.Escape:
                    return;
                case ConsoleKey.Q:
                    return;
            }
        }
    }

    static void DisplayCalendar(DateTime date)
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

        DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

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

            var currentDay = new DateTime(date.Year, date.Month, day);
            if (day == date.Day)
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
                .Header($"[bold green]{date:MMMM yyyy}[/]", Justify.Center)
                .BorderColor(Color.Blue)
        );

        AnsiConsole.Write(
            new Panel($"[bold green]You selected: {date.ToShortDateString()}[/]")
                .Header("test header", Justify.Center)
                .BorderColor(Color.Red)
        );
    }
}
