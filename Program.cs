namespace PersonnelHolidayPlanner;

class Program
{
    // intial view
    static Views.AppState state = Views.AppState.CALENDAR;
    public static DBContext.PHPContext context = new DBContext.PHPContext();

    static void Main(string[] args)
    {
        Initialize();

        while (true)
        {
            RenderViews();

            HandleKeys();
        }
    }

    public static void Initialize()
    {
        // Console.WriteLine(context.Leaves.First().LeaveType);
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
