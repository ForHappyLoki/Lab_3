using System;
using System.Collections.Generic;

namespace Lab_3.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string GenreName { get; set; } = null!;

    public string? GenreDescription { get; set; }

    public virtual ICollection<Tvshow> Tvshows { get; set; } = new List<Tvshow>();
}
