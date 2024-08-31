using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodingTracker.Data.EntityContexts;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Data.Interfaces;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.DataInterfaces.IGenericRepository;
using System.Diagnostics;
using System.Security.Cryptography;
using CodingTracker.Logging.ApplicationLoggers;


namespace CodingTracker.Data.Repositories.GenericRepository
{
    public class GenericRepository<T, TId> : IGenericRepository<T, TId> where T : class  //TId is the primary key for the entity (SessionId/ UserId)
    {
        protected readonly EntityContext _context;
        protected readonly DbSet<T> _dbSet;
        protected readonly IApplicationLogger _appLogger;

        public GenericRepository(EntityContext context, IApplicationLogger applogger)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _appLogger = applogger;
        }
        public virtual async Task<T> GetByIdAsync(Activity activity, TId id) // Virtual means that the method can be overwritten in any derived class. 
        {
            _appLogger.Info($"Starting {nameof(GetByIdAsync)} for {typeof(T).Name} with ID {id}. TraceId: {activity.TraceId}.");
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _appLogger.Info($"Successfully retrieved {typeof(T).Name} with ID {id}. TraceId: {activity.TraceId}.");
                }
                else
                {
                    _appLogger.Info($"No {typeof(T).Name} found with ID {id}. TraceId: {activity.TraceId}.");
                }
                return entity;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving {typeof(T).Name} with ID {id} for {nameof(GetByIdAsync)}, TraceId: {activity.TraceId}.", ex);
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(Activity activity)
        {
            _appLogger.Info($"Starting {nameof(GetAllAsync)} for {typeof(T).Name}. TraceId: {activity.TraceId}.");
            try
            {
                var entities = await _dbSet.ToListAsync();
                _appLogger.Info($"Successfully retrieved {entities.Count} {typeof(T).Name}(s). TraceId: {activity.TraceId}.");
                return entities;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error retrieving all {typeof(T).Name}s for {nameof(GetAllAsync)}, TraceId: {activity.TraceId}.", ex);
                return Enumerable.Empty<T>();
            }
        }

        public virtual async Task<bool> AddAsync(Activity activity, T entity)
        {
            _appLogger.Info($"Starting {nameof(AddAsync)} for {typeof(T).Name}. TraceId: {activity.TraceId}.");
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                _appLogger.Info($"Successfully added new {typeof(T).Name}. TraceId: {activity.TraceId}.");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error adding new {typeof(T).Name} for {nameof(AddAsync)}, TraceId: {activity.TraceId}.", ex);
                return false;
            }
        }

        public virtual async Task<bool> UpdateAsync(Activity activity, T entity)
        {
            _appLogger.Info($"Starting {nameof(UpdateAsync)} for {typeof(T).Name}. TraceId: {activity.TraceId}.");
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                _appLogger.Info($"Successfully updated {typeof(T).Name}. TraceId: {activity.TraceId}.");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error updating {typeof(T).Name} for {nameof(UpdateAsync)}, TraceId: {activity.TraceId}.", ex);
                return false;
            }
        }

        public virtual async Task<bool> DeleteAsync(Activity activity, TId id)
        {
            _appLogger.Info($"Starting {nameof(DeleteAsync)} for {typeof(T).Name} with ID {id}. TraceId: {activity.TraceId}.");
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    _appLogger.Info($"No {typeof(T).Name} found with ID {id} for deletion. TraceId: {activity.TraceId}.");
                    return false;
                }

                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                _appLogger.Info($"Successfully deleted {typeof(T).Name} with ID {id}. TraceId: {activity.TraceId}.");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error deleting {typeof(T).Name} with ID {id} for {nameof(DeleteAsync)}, TraceId: {activity.TraceId}.", ex);
                return false;
            }
        }


    }
}
