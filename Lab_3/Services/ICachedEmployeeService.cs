using System.Threading.Tasks;
using Lab_3.Models;

namespace Lab_3.Services
{
    public interface ICachedEmployeeService
    {
        public IEnumerable<Employee> GetEmployee(int rowsNumber = 20);
        public void AddEmployee(string cacheKey, int rowsNumber = 20);
        public IEnumerable<Employee> GetEmployee(string cacheKey, int rowsNumber = 20);
    }
}
