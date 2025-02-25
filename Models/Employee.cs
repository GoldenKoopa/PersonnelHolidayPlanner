using System;
using System.Collections.Generic;

namespace LeaveCliTool.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
