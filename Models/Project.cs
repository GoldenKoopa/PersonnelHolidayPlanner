using System;
using System.Collections.Generic;

namespace PersonnelHolidayPlanner.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
