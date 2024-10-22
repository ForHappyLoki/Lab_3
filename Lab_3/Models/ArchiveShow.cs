using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class ArchiveShow
{
    public int ArchiveId { get; set; }

    public int ShowId { get; set; }

    public string ShowName { get; set; } = null!;

    public int GenreId { get; set; }

    public int Duration { get; set; }

    public DateTime AirDate { get; set; }

    public string? Description { get; set; }

    public virtual Tvshow Show { get; set; } = null!;
}
