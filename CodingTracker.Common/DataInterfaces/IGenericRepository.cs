using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.DataInterfaces.IGenericRepository
{
    public interface IGenericRepository<T, TId> where T : class
    {

        Task<T> GetByIdAsync(Activity activity, TId id);
        Task<IEnumerable<T>> GetAllAsync(Activity activity);
        Task<bool> AddAsync(Activity activity, T entity);
        Task<bool> UpdateAsync(Activity activity, T entity);
        Task<bool> DeleteAsync(Activity activity, TId id);

    }
}
