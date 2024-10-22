using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class Broadcasting
{
    public int BroadcastingId { get; set; }

    public int EmployeeId { get; set; }

    public int ShowId { get; set; }

    public int WorkHours { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Tvshow Show { get; set; } = null!;
}
