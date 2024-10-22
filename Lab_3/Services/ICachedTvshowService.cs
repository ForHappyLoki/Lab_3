using System.Threading.Tasks;
using Lab_3.Models;

namespace Lab_3.Services
{
    public interface ICachedTvshowService
    {
        public IEnumerable<Tvshow> GetTvshow(int rowsNumber = 20);
        public void AddTvshow(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Tvshow> GetTvshow(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Tvshow> FindTvshow(string cacheKey);
    }
}
