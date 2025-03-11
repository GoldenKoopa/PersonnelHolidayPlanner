namespace PersonnelHolidayPlanner;

public class Utils
{
    public static Dictionary<DateOnly, List<string>> createMonth(
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
                if (employeesOnLeave == project.Employees.Count() - 1)
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
}
