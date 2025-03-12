using System.Text.Json;
using DotNetEnv;

namespace PersonnelHolidayPlanner;

public class Headless
{
    private static int employeeId = -1;
    private static DateOnly firstDay;
    private static DateOnly lastDay;
    private static bool json;

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
        replaceOptionalValues();
        validateArguments();
        Dictionary<DateOnly, List<string>> result = Utils.generateTimeframe(
            Headless.employeeId,
            Headless.firstDay,
            Headless.lastDay
        );

        if (Headless.json)
        {
            PrintJsonTimeframe(result);
        }
        else
        {
            PrettyPrintTimeframe(result);
        }
    }

    public static void PrintJsonTimeframe(Dictionary<DateOnly, List<string>> timeframe)
    {
        if (timeframe == null || timeframe.Count == 0)
        {
            Console.WriteLine("{}");
            return;
        }

        var jsonObject = new Dictionary<string, object>();

        foreach (var entry in timeframe)
        {
            jsonObject[entry.Key.ToString("yyyy-MM-dd")] = new
            {
                project_count = entry.Value.Count,
                projects = entry.Value,
            };
        }

        var options = new JsonSerializerOptions { WriteIndented = true };

        string jsonString = JsonSerializer.Serialize(jsonObject, options);
        Console.WriteLine(jsonString);
    }

    public static void PrettyPrintTimeframe(Dictionary<DateOnly, List<string>> timeframe)
    {
        if (timeframe == null || timeframe.Count == 0)
        {
            Console.WriteLine("NO_DATA");
            return;
        }

        // Print header
        Console.WriteLine("DATE\t\tPROJECT_COUNT\tPROJECTS");

        foreach (var entry in timeframe.OrderBy(e => e.Key))
        {
            string dateStr = entry.Key.ToString("yyyy-MM-dd");
            string projectCount = entry.Value.Count.ToString();
            string projects = string.Join("|", entry.Value);

            Console.WriteLine($"{dateStr}\t{projectCount}\t\t{projects}");
        }
    }

    private static void replaceOptionalValues()
    {
        if (Headless.employeeId != -1)
        {
            return;
        }

        Env.Load();
        if (int.TryParse(Environment.GetEnvironmentVariable("employee_id"), out int employeeId))
        {
            Headless.employeeId = employeeId;
            return;
        }
        Console.WriteLine("Default employeeId in is not a valid integer");
        Environment.Exit(0);
    }

    private static void validateArguments()
    {
        if (Headless.firstDay == DateOnly.MinValue || Headless.lastDay == DateOnly.MinValue)
        {
            Console.WriteLine("First and/or last day are not set");
            Environment.Exit(0);
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
            case "--json":
                Headless.json = true;
                return;
            case "--help":
            case "-h":
                Console.WriteLine(HelpMessage);
                Environment.Exit(0);
                return; // get rid of cannot fall through warning
            default:
                Console.WriteLine($"Invalid argument {arg}");
                Environment.Exit(0);
                return; // again, warning
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
