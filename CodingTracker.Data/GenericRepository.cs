using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Data.IGenericRepository;
using CodingTracker.Data.EntityContexts;
using Microsoft.EntityFrameworkCore;


namespace CodingTracker.Data.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly EntityContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(EntityContext context)
        {
            _context = context;
            _dbSet = context.Set<T>(); // Represents a collection of all the entities (CodingSession & UserCredentials) that can be queried.
        }

        public async Task<T> GetIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
              _dbSet.Update(entity);
             await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if(entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }



    }
}
