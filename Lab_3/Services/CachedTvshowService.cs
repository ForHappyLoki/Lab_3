using Lab_3.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Lab_3.Services
{
    public class CachedTvshowService(DatabaseContext dbContext, IMemoryCache memoryCache) : ICachedTvshowService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly IMemoryCache _memoryCache = memoryCache;

        // получение списка емкостей из базы
        public IEnumerable<Tvshow> GetTvshow(int rowsNumber = 20)
        {
            return _dbContext.Tvshows.Take(rowsNumber).ToList();
        }

        // добавление списка емкостей в кэш
        public void AddTvshow(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Tvshow> tvshow = _dbContext.Tvshows.Take(rowsNumber).ToList();
            if (tvshow != null)
            {
                _memoryCache.Set(cacheKey, tvshow, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(250)
                });

            }

        }
        // получение списка емкостей из кэша или из базы, если нет в кэше
        public IEnumerable<Tvshow> GetTvshow(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Tvshow> tvshow))
            {
                tvshow = _dbContext.Tvshows.Take(rowsNumber).ToList();
                if (tvshow != null)
                {
                    _memoryCache.Set(cacheKey, tvshow,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(250)));
                }
            }
            return tvshow;
        }

        public IEnumerable<Tvshow> FindTvshow(string cacheKey)
        {
            List<Tvshow> result = new List<Tvshow>();
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Tvshow> show))
            {
                show = _dbContext.Tvshows.ToList();
                if (show != null)
                {
                    _memoryCache.Set(cacheKey, show,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(250)));
                }
            }
            if (show != null)
            {
                foreach (var s in show)
                {
                    if (s.ShowName == cacheKey)
                    {
                        result.Add(s);
                    }
                }
            }
            return result;
        }
    }
}
