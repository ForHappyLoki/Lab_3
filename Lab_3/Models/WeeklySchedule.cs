using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class WeeklySchedule
{
    public int ScheduleId { get; set; }

    public int WeekNumber { get; set; }

    public int Month { get; set; }

    public int Year { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int ShowId { get; set; }

    public string? Guests { get; set; }

    public virtual Tvshow Show { get; set; } = null!;
}
