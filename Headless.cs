using System.Text.Json;
using DotNetEnv;

namespace PersonnelHolidayPlanner;

public class Headless
{
    private static int employeeId = -1;
    private static DateOnly firstDay;
    private static DateOnly lastDay;
    private static bool json;
    private static bool leaves;
    private static string outputFile = "output.xlsx";
    private static bool export;

    private const string HelpMessage =
        @"
Personnel Holiday Planner - Command Line Interface

Usage:
  PersonnelHolidayPlanner [OPTIONS]

Options:
  --employee ID     Optional employee ID (integer). If not provided, reads from environment variable 'employee_id'.
  --from DATE       Start date (required, format: YYYY-MM-DD)
  --to DATE         End date (required, format: YYYY-MM-DD)
  --leaves          Optional flag to only show leave dates.
  --json            Optional flag to output data in JSON format.
  --export          Optional flag to export data to Excel file.
  -o, --output FILE Optional output file name for Excel export (default: output.xlsx)
  -h, --help        Show this help message

Example:
  PersonnelHolidayPlanner --employee 42 --from 2025-06-01 --to 2025-06-15 --leaves --json

Note:
  Dates should follow ISO-8601 format (YYYY-MM-DD) for best compatibility.
  Alternative local formats may work but are not guaranteed.
  If --employee is not provided, the 'employee_id' environment variable will be used.

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
        if (Headless.leaves)
        {
            handleLeaves();
        }
        else
        {
            handleBlockedDays();
        }
    }

    public static void handleBlockedDays()
    {
        if (Headless.export)
        {
            try
            {
                Utils.ExportTimeframeToExcel(
                    Utils.generateTimeframe(
                        Headless.employeeId,
                        Headless.firstDay,
                        Headless.lastDay
                    ),
                    Headless.outputFile
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"export failed: {e.Message}");
            }
            return;
        }

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

    public static void handleLeaves()
    {
        if (Headless.export)
        {
            try
            {
                Utils.ExportLeavesToExcel(
                    Utils.getEmployeeLeaveDatesWithTypes(
                        Headless.employeeId,
                        Headless.firstDay,
                        Headless.lastDay
                    ),
                    Headless.outputFile
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"export failed: {e.Message}");
            }
            return;
        }

        HashSet<DateOnly> resultLeaves = Utils.getEmployeeLeaveDates(
            Headless.employeeId,
            Headless.firstDay,
            Headless.lastDay
        );

        List<DateOnly> sortedLeaves = resultLeaves.OrderBy(d => d).ToList();
        if (Headless.json)
        {
            PrintJsonLeaves(sortedLeaves);
        }
        else
        {
            PrettyPrintLeaves(sortedLeaves);
        }
        return;
    }

    public static void PrettyPrintLeaves(List<DateOnly> leaves)
    {
        if (leaves == null || leaves.Count == 0)
        {
            Console.WriteLine("NO_DATA");
            return;
        }

        Console.WriteLine("LEAVES");

        foreach (DateOnly date in leaves)
        {
            Console.WriteLine(date.ToString("yyyy-MM-dd"));
        }
    }

    public static void PrintJsonLeaves(List<DateOnly> leaves)
    {
        if (leaves == null || leaves.Count == 0)
        {
            Console.WriteLine("{}");
            return;
        }

        List<string> result = new();

        foreach (DateOnly date in leaves)
        {
            result.Add(date.ToString("yyyy-MM-dd"));
        }

        JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        string jsonString = JsonSerializer.Serialize(result, jsonOptions);
        Console.WriteLine(jsonString);
    }

    public static void PrintJsonTimeframe(Dictionary<DateOnly, List<string>> timeframe)
    {
        if (timeframe == null || timeframe.Count == 0)
        {
            Console.WriteLine("{}");
            return;
        }

        Dictionary<string, object> jsonObject = new();

        foreach (KeyValuePair<DateOnly, List<string>> entry in timeframe)
        {
            jsonObject[entry.Key.ToString("yyyy-MM-dd")] = new
            {
                project_count = entry.Value.Count,
                projects = entry.Value,
            };
        }

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

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

        foreach (KeyValuePair<DateOnly, List<string>> entry in timeframe.OrderBy(e => e.Key))
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
                break;
            case "--from":
                trySetStartDate(arguments);
                break;
            case "--to":
                trySetEndDate(arguments);
                break;
            case "--leaves":
                Headless.leaves = true;
                break;
            case "--json":
                Headless.json = true;
                break;
            case "--export":
                Headless.export = true;
                break;
            case "-o":
            case "--output":
                trySetOutput(arguments);
                break;
            case "--help":
            case "-h":
                Console.WriteLine(HelpMessage);
                Environment.Exit(0);
                break; // get rid of cannot fall through warning
            default:
                Console.WriteLine($"Invalid argument {arg}");
                Environment.Exit(0);
                break; // again, warning
        }
    }

    public static void trySetOutput(List<string> arguments)
    {
        if (arguments.Count() == 0 || arguments[0].StartsWith("--"))
        {
            Console.WriteLine("missing value for argument '--output'/'-o'");
            Environment.Exit(0);
        }
        Headless.outputFile = arguments[0];
        arguments.RemoveAt(0);
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
