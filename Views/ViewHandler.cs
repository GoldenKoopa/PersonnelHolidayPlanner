namespace PersonnelHolidayPlanner.Views;

public enum AppState
{
    CALENDAR,
}

public class Views
{
    public static Dictionary<AppState, View> map = new Dictionary<AppState, View>
    {
        { AppState.CALENDAR, new CalendarView() },
    };
}
