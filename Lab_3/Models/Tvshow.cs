using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class Tvshow
{
    public int ShowId { get; set; }

    public string ShowName { get; set; } = null!;

    public int Duration { get; set; }

    public decimal? Rating { get; set; }

    public int GenreId { get; set; }

    public string? Description { get; set; }

    public string? Employees { get; set; }

    public virtual ICollection<ArchiveShow> ArchiveShows { get; set; } = new List<ArchiveShow>();

    public virtual ICollection<Broadcasting> Broadcastings { get; set; } = new List<Broadcasting>();

    public virtual ICollection<CitizenAppeal> CitizenAppeals { get; set; } = new List<CitizenAppeal>();

    public virtual Genre Genre { get; set; } = null!;

    public virtual ICollection<WeeklySchedule> WeeklySchedules { get; set; } = new List<WeeklySchedule>();
}
