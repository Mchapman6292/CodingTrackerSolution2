using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Data.IGenericRepository
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T> GetIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);
        Task UpdateAsync(T entity);  // Returning Task indicates Async.
        Task DeleteAsync(int id);
         



    }
}
