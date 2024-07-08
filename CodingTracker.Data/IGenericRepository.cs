using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task ReadAsync(T entity);

        Task<T> GetIdAsync(int id);

        Task<T> GetAllAsync();

    }
}
