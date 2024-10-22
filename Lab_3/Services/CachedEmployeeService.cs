using Lab_3.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Lab_3.Services
{
    public class CachedEmployeeService(DatabaseContext dbContext, IMemoryCache memoryCache) : ICachedEmployeeService
    {
        private readonly DatabaseContext _dbContext = dbContext;
        private readonly IMemoryCache _memoryCache = memoryCache;

        // получение списка емкостей из базы
        public IEnumerable<Employee> GetEmployee(int rowsNumber = 20)
        {
            return _dbContext.Employees.Take(rowsNumber).ToList();
        }

        // добавление списка емкостей в кэш
        public void AddEmployee(string cacheKey, int rowsNumber = 20)
        {
            IEnumerable<Employee> employee = _dbContext.Employees.Take(rowsNumber).ToList();
            if (employee != null)
            {
                _memoryCache.Set(cacheKey, employee, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(250)
                });

            }

        }
        // получение списка емкостей из кэша или из базы, если нет в кэше
        public IEnumerable<Employee> GetEmployee(string cacheKey, int rowsNumber = 20)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<Employee> employee))
            {
                employee = _dbContext.Employees.Take(rowsNumber).ToList();
                if (employee != null)
                {
                    _memoryCache.Set(cacheKey, employee,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(250)));
                }
            }
            return employee;
        }

    }
}
