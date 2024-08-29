using CodingTracker.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Data.Interfaces.IUserCredentialRepository;
using CodingTracker.Data.EntityContexts;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Data.Repositories.GenericRepository;
using CodingTracker.Common.IAuthenticationServices;
using CodingTracker.Common.ILoginManagers;
using System.Diagnostics;

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

        public async Task<UserCredential> GetCredentialByUsername(string username, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(GetCredentialByUsername)} TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}.");
            try
            {
                var user = await _dbSet.FirstOrDefaultAsync(uc => uc.Username == username);

                if (user == null)
                {
                    _appLogger.Info($"User not found for username: {username}. TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}.");
                }
                else
                {
                    _appLogger.Info($"User found for username: {username}. TraceId: {activity.TraceId}  , ParentId:   {activity.ParentId}.");
                }
                return user;

            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(GetCredentialByUsername)} for username: {username}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.", ex);
                return null;
            }
        }


        public async Task<bool> CreateUserCredential(UserCredential newUser, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(CreateUserCredential)} TraceId: {activity.TraceId}, ParentId");

            try
            {
                await _dbSet.AddAsync(newUser);

                await _context.SaveChangesAsync();
                _appLogger.Info($"Successfully created new user credential for username: {newUser.Username}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error creating new user credential for username: {newUser.Username}. TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}", ex);
                return false;
            }
        }


        public async Task UpdateUserCredentialPassword(string username, string newPassword, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(UpdateUserCredentialPassword)} TraceId: {activity.TraceId} , ParentId:   {activity.ParentId}.");

            try
            {
                UserCredential user = await GetCredentialByUsername(username, activity);

                string hashedNewPassword = _authService.HashPassword(newPassword, activity);

                user.PasswordHash = hashedNewPassword;

                await _context.SaveChangesAsync();

                _appLogger.Info($"Password updated successfully for User: {username},TraceId: {activity.TraceId}  , ParentId:{activity.ParentId}.");
            }
            catch (DbUpdateException dbEx)
            {
                _appLogger.Error($"Database error while updating password for user: {username}. TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error updating password for user: {username}. TraceId: {activity.TraceId}  , ParentId:   {activity.ParentId}", ex);
                throw;
            }
        }


        public async Task<bool> UpdateLastLogin(string username, DateTime lastLogin, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(UpdateLastLogin)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}");

            try
            {
                UserCredential user = await GetCredentialByUsername(username, activity);

                user.LastLogin = lastLogin;

                await _context.SaveChangesAsync();

                _appLogger.Info($"LastLogin updated successfully for User: {username},TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}.");
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _appLogger.Error($"Database error while updating LastLogin for user: {username}. TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}", dbEx);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error updating LastLogin for user: {username}. TraceId: {activity.TraceId}  , ParentId:   {activity.ParentId}", ex);
                throw;
            }
        }
    }
}