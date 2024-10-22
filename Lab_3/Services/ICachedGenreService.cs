using System.Threading.Tasks;
using Lab_3.Models;

namespace Lab_3.Services
{
    public interface ICachedGenreService
    {
        public IEnumerable<Genre> GetGenre(int rowsNumber = 20);
        public void AddGenre(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Genre> GetGenre(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Genre> FindGenre(string cacheKey);
    }
}
