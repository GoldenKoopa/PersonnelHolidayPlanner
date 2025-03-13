using Spectre.Console;

namespace PersonnelHolidayPlanner.Views;

public class HelpView : View
{
    public void renderView()
    {
        Table table = new();
        table.AddColumn("Key");
        table.AddColumn("Action");

        // keybinds
        table.AddRow("LEFT/h", "move selection to the right");
        table.AddRow("DOWN/j", "move selection down");
        table.AddRow("UP/k", "move selection up");
        table.AddRow("RIGHT/l", "move selection to the right");
        table.AddRow("SHIFT+UP/p", "go to previous month");
        table.AddRow("SHIFT+DOWN/n", "go to next month");
        table.AddRow("e", "export blocked days of the current year");
        table.AddRow("SHIFT+e", "export leave days of the current year");
        table.AddRow("q/ESC", "quit the application");
        table.AddRow("", "");
        table.AddRow("?", "return to calendar view");

        AnsiConsole.Write(new Panel(table));
    }

    public void handleKeys(ConsoleKeyInfo keyInfo)
    {

        if (keyInfo.Key == ConsoleKey.Q) {
            Environment.Exit(0);
        }
        if (keyInfo.KeyChar == '?') {
            Program.state = AppState.CALENDAR;
        }
    }
}
