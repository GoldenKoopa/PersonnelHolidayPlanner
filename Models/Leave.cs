using System;
using System.Collections.Generic;

namespace PersonnelHolidayPlanner.Models;

public partial class Leave
{
    public int LeaveId { get; set; }

    public int? EmployeeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string LeaveType { get; set; } = null!;

    public virtual Employee? Employee { get; set; }
}
