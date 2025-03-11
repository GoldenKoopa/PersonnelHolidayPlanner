namespace PersonnelHolidayPlanner;

public class Headless
{
    private static int employeeId;
    private static DateOnly firstDay;
    private static DateOnly lastDay;

    private const string HelpMessage =
        @"
Personnel Holiday Planner - Command Line Interface

Usage:
  PersonnelHolidayPlanner [OPTIONS]

Options:
  --employee ID     Optional employee ID (integer)
  --from DATE       Start date (required, format: YYYY-MM-DD)
  --to DATE         End date (required, format: YYYY-MM-DD)
  -h, --help        Show this help message

Example:
  PersonnelHolidayPlanner --employee 42 --from 2025-06-01 --to 2025-06-15

Note:
  Dates should follow ISO-8601 format (YYYY-MM-DD) for best compatibility.
  Alternative local formats may work but are not guaranteed.
";

    public static void run(string[] args)
    {
        List<string> arguments = new(args);

        while (arguments.Count() > 0)
        {
            handleArgument(arguments);
        }
    }

    public static void handleArgument(List<string> arguments)
    {
        string arg = arguments[0];
        arguments.RemoveAt(0);
        // Console.WriteLine(arg);

        switch (arg)
        {
            case "--employee": // optional
                trySetEmployee(arguments);
                return;
            case "--from":
                trySetStartDate(arguments);
                return;
            case "--to":
                trySetEndDate(arguments);
                return;
            case "--help":
            case "-h":
                Console.WriteLine(HelpMessage);
                Environment.Exit(0);
                return; // get rid of cannot fall through final warning
        }
    }

    public static void trySetEndDate(List<string> arguments)
    {
        if (arguments.Count() == 0 || arguments[0].StartsWith("--"))
        {
            Console.WriteLine("missing value for argument '--to'");
            Environment.Exit(0);
        }
        String lastDayString = arguments[0];
        arguments.RemoveAt(0);
        if (DateOnly.TryParse(lastDayString, out DateOnly lastDay))
        {
            Headless.lastDay = lastDay;
        }
        else
        {
            Console.WriteLine(
                "value of argument '--to' is not a valid "
                    + "date\nplease use ISO-8601 (YYYY-MM-DD) or another valid format"
            );
            Environment.Exit(0);
        }
    }

    public static void trySetStartDate(List<string> arguments)
    {
        if (arguments.Count() == 0 || arguments[0].StartsWith("--"))
        {
            Console.WriteLine("missing value for argument '--from'");
            Environment.Exit(0);
        }
        String firstDayString = arguments[0];
        arguments.RemoveAt(0);
        if (DateOnly.TryParse(firstDayString, out DateOnly firstDay))
        {
            Headless.firstDay = firstDay;
        }
        else
        {
            Console.WriteLine(
                "value of argument '--from' is not a valid "
                    + "date\nplease use ISO-8601 (YYYY-MM-DD) or another valid format"
            );
            Environment.Exit(0);
        }
    }

    public static void trySetEmployee(List<string> arguments)
    {
        if (arguments.Count() == 0 || arguments[0].StartsWith("--"))
        {
            Console.WriteLine("missing value for argument '--employee'");
            Environment.Exit(0);
        }
        String employeeIdString = arguments[0];
        arguments.RemoveAt(0);
        if (int.TryParse(employeeIdString, out int employeeId))
        {
            Headless.employeeId = employeeId;
        }
        else
        {
            Console.WriteLine("value of argument '--employee' is not of type int");
            Environment.Exit(0);
        }
    }
}
