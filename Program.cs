using Microsoft.EntityFrameworkCore;

namespace PersonnelHolidayPlanner;

class Program
{
    public static Views.AppState state;
    public static DBContext.PHPContext? context;

    static void Main(string[] args)
    {
        Initialize();

        if (args.Length != 0)
        {
            Headless.run(args);
            return;
        }

        // not headless
        while (true)
        {
            RenderViews();

            HandleKeys();
        }
    }

    public static void Initialize()
    {
        try
        {
            context = new DBContext.PHPContext();

            // test connection
            context.Database.OpenConnection();
            context.Database.CloseConnection();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Database connection failed. {e.Message}");
            Environment.Exit(0);
        }
        state = Views.AppState.CALENDAR;
    }

    public static void RenderViews()
    {
        Console.Clear();
        if (Views.Views.map.TryGetValue(state, out Views.View? currentView))
        {
            currentView.renderView();
        }
        else
        {
            throw new Exception("invalid state");
        }
    }

    public static void HandleKeys()
    {
        ConsoleKeyInfo keyInfo = Console.ReadKey(true);
        if (Views.Views.map.TryGetValue(state, out Views.View? currentView))
        {
            currentView.handleKeys(keyInfo);
        }
        else
        {
            throw new Exception("invalid state");
        }
    }
}
