using Lab_3.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Lab_3.Services
{
    public class CachedGenreService(DatabaseContext dbContext, IMemoryCache memoryCache) : ICachedGenreService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly IMemoryCache _memoryCache = memoryCache;

        // получение списка емкостей из базы
        public IEnumerable<Genre> GetGenre(int rowsNumber = 20)
        {
            return _dbContext.Genres.Take(rowsNumber).ToList();
        }

        // добавление списка емкостей в кэш
        public void AddGenre(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Genre> tanks = _dbContext.Genres.Take(rowsNumber).ToList();
            if (tanks != null)
            {
                _memoryCache.Set(cacheKey, tanks, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(250)
                });

            }

        }
        // получение списка емкостей из кэша или из базы, если нет в кэше
        public IEnumerable<Genre> GetGenre(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Genre> genre))
            {
                genre = _dbContext.Genres.Take(rowsNumber).ToList();
                if (genre != null)
                {
                    _memoryCache.Set(cacheKey, genre,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(250)));
                }
            }
            return genre;
        }
        // получение списка емкостей из кэша или из базы, если нет в кэше
        public IEnumerable<Genre> FindGenre(string cacheKey)
        {
            List<Genre> result = new List<Genre>();
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Genre> genre))
            {
                genre = _dbContext.Genres.ToList();
                if (genre != null)
                {
                    _memoryCache.Set(cacheKey, genre,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(250)));
                }
            }
            if (genre != null)
            {
                foreach(var g in genre)
                {
                    if (g.GenreName == cacheKey)
                    {
                        result.Add(g);
                    }
                }
            }    
            return result;
        }

    }
}
