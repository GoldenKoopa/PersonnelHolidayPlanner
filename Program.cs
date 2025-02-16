namespace LeaveCli;

class Program
{
    static Views.AppState state = Views.AppState.CALENDAR;

    static DateTime currentDate = DateTime.Now;

    static void Main(string[] args)
    {
        // Initalize();

        while (true)
        {
            RenderViews();

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            HandleKeys(keyInfo);
        }
    }

    // public static void Initialize() { }

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

    public static void HandleKeys(ConsoleKeyInfo keyInfo)
    {
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
