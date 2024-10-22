namespace Lab_3.Models
{
    public class Search
    {
        public string SearchRequest { get; set; }
        public IEnumerable<Genre> AnswerGenre { get; set; }
        public IEnumerable<Tvshow> AnswerTvshow { get; set; }
    }
}
