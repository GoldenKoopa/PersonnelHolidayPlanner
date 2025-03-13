namespace PersonnelHolidayPlanner.Views;

public enum AppState
{
    CALENDAR,
    HELP,
}

public class Views
{
    public static Dictionary<AppState, View> map = new Dictionary<AppState, View>
    {
        { AppState.CALENDAR, new CalendarView() },
        { AppState.HELP, new HelpView() },
    };
}
