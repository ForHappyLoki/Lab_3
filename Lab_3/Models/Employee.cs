using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public virtual ICollection<Broadcasting> Broadcastings { get; set; } = new List<Broadcasting>();
}
