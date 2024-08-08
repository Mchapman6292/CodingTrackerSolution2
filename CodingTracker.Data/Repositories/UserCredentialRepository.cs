using CodingTracker.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Data.Interfaces.IUserCredentialRepository;
using CodingTracker.Data.EntityContexts;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Data.Repositories.GenericRepository;
using CodingTracker.Common.IAuthenticationServices;
using CodingTracker.Common.ILoginManagers;

namespace CodingTracker.Data.Repositories.UserCredentialRepository
{
    public class UserCredentialRepository : GenericRepository<UserCredential>, IUserCredentialRepository
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IAuthenticationService _authService;

        public UserCredentialRepository(EntityContext context, IApplicationLogger appLogger, IAuthenticationService authService) : base(context)

        {
            _appLogger = appLogger;
            _authService = authService;
        }

        public async Task<UserCredential> GetCredentialByUsername(string username, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(GetCredentialByUsername)} TraceId: {traceId}, ParentId: {parentId}.");
            try
            {
                var user = await _dbSet.FirstOrDefaultAsync(uc => uc.Username == username);

                if (user == null)
                {
                    _appLogger.Info($"User not found for username: {username}. TraceId: {traceId}, ParentId: {parentId}.");
                }
                else
                {
                    _appLogger.Info($"User found for username: {username}. TraceId: {traceId}, ParentId: {parentId}.");
                }
                return user;

            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(GetCredentialByUsername)} for username: {username}. TraceId: {traceId}, ParentId: {parentId}.", ex);
                return null;
            }
        }


        public async Task<bool> CreateUserCredential(UserCredential newUser, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(CreateUserCredential)} TraceId: {traceId}, ParentId");

            try
            {
                await _dbSet.AddAsync(newUser);

                await _context.SaveChangesAsync();
                _appLogger.Info($"Successfully created new user credential for username: {newUser.Username}. TraceId: {traceId}, ParentId: {parentId}");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error creating new user credential for username: {newUser.Username}. TraceId: {traceId}, ParentId: {parentId}", ex);
                return false;
            }
        }


        public async Task UpdateUserCredentialPassword(string username, string newPassword, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(UpdateUserCredentialPassword)} TraceId: {traceId}, ParentId: {parentId}.");

            try
            {
                UserCredential user = await GetCredentialByUsername(username, traceId, parentId);

                string hashedNewPassword = _authService.HashPassword(newPassword, traceId, parentId);

                user.PasswordHash = hashedNewPassword;

                await _context.SaveChangesAsync();

                _appLogger.Info($"Password updated successfully for User: {username},TraceId: {traceId}, ParentId: {parentId}.");
            }
            catch (DbUpdateException dbEx)
            {
                _appLogger.Error($"Database error while updating password for user: {username}. TraceId: {traceId}, ParentId: {parentId}", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error updating password for user: {username}. TraceId: {traceId}, ParentId: {parentId}", ex);
                throw;
            }
        }


        public async Task<bool> UpdateLastLogin(string username, DateTime lastLogin, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(UpdateLastLogin)} TraceId: {traceId}, ParentId: {parentId}");

            try
            {
                UserCredential user = await GetCredentialByUsername(username, traceId, parentId);

                user.LastLogin = lastLogin;

                await _context.SaveChangesAsync();

                _appLogger.Info($"LastLogin updated successfully for User: {username},TraceId: {traceId}, ParentId: {parentId}.");
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _appLogger.Error($"Database error while updating LastLogin for user: {username}. TraceId: {traceId}, ParentId: {parentId}", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error updating LastLogin for user: {username}. TraceId: {traceId}, ParentId: {parentId}", ex);
                throw;
            }
        }
    }
}