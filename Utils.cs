namespace PersonnelHolidayPlanner;

public class Utils
{
    public static Dictionary<DateOnly, List<string>> generateTimeframe(
        int employeeId,
        DateOnly firstDay,
        DateOnly lastDay
    )
    {
        Dictionary<DateOnly, List<string>> result = new Dictionary<DateOnly, List<string>>();
        var projects = Program.context.Employees.Find(employeeId)!.Projects;

        foreach (Models.Project project in projects)
        {
            var employeeCount = project.Employees.Count();

            for (DateOnly date = firstDay; date <= lastDay; date = date.AddDays(1))
            {
                int employeesOnLeave = project
                    .Employees.Where(e =>
                        e.Leaves.Any(l => l.StartDate <= date && l.EndDate >= date)
                    )
                    .Count();
                if (
                    employeesOnLeave == project.Employees.Count() - 1
                    && !hasLeaveOnDate(employeeId, date)
                )
                {
                    if (result.TryGetValue(date, out List<string>? projectList))
                    {
                        projectList.Add(project.Name);
                    }
                    else
                    {
                        result.Add(date, new List<string>() { project.Name });
                    }
                }
            }
        }

        return result;
    }

    public static bool hasLeaveOnDate(int employeeId, DateOnly date)
    {
        return Program.context.Leaves.Any(leave =>
            leave.EmployeeId == employeeId && leave.StartDate <= date && leave.EndDate >= date
        );
    }
}
