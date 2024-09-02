using CodingTracker.Common.DataInterfaces;
using Microsoft.EntityFrameworkCore;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepository;
using CodingTracker.Data.EntityContexts;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Common.IAuthenticationServices;
using CodingTracker.Common.IAuthtenticationServices;
using CodingTracker.Common.DataInterfaces.IEntityContexts;
using CodingTracker.Common.IUtilityServices;
using System.Diagnostics;

namespace CodingTracker.Data.Repositories.UserCredentialRepository
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IEntityContext _context;
        private readonly IUtilityService _utilityService;

        public UserCredentialRepository(IApplicationLogger appLogger, IEntityContext context, IUtilityService utilityService)

        {
            _appLogger = appLogger;
            _context = context;
            _utilityService = utilityService;
        }

        public async Task<UserCredential> GetCredentialByUsername(string username, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(GetCredentialByUsername)} TraceId: {activity.TraceId} , ParentId:  {activity.ParentId}.");
            try
            {
                var user = await _context.UserCredentials.FirstOrDefaultAsync(uc => uc.Username == username);

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


        public async Task<bool> AddUserCredential(Activity activity, UserCredential newUser)
        {
            _appLogger.Info($"Starting {nameof(AddUserCredential)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}");
            try
            {
                await _context.UserCredentials.AddAsync(newUser);
                int rowsAffected = await _context.SaveChangesAsync();

                if (rowsAffected > 0) 
                {
                    _appLogger.Info($"Successfully created new user credential for username: {newUser.Username}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}");
                    return true;
                }
                else 
                {
                    _appLogger.Warning($"User credential for username: {newUser.Username} was not added to the database. No rows affected. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}");
                    return false;
                }
            }
            catch (DbUpdateException dbEx)
            {
                _appLogger.Error($"Database error creating new user credential for username: {newUser.Username}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", dbEx);
                return false;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Unexpected error creating new user credential for username: {newUser.Username}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", ex);
                return false;
            }
        }


        public async Task UpdateUserCredentialPassword(string username, string password, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(UpdateUserCredentialPassword)} TraceId: {activity.TraceId} , ParentId:   {activity.ParentId}.");

            try
            {
                UserCredential user = await GetCredentialByUsername(username, activity);

                string hashedNewPassword = _utilityService.HashPassword( activity, password);

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

        public async Task<bool> SaveUserCredentialChanges(Activity activity)
        {
            try
            {
                await _context.SaveChangesAsync();
                _appLogger.Info($" SaveUserCredentialChanges successful TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                return true;
            }
            catch (DbUpdateException ex)
            {
                _appLogger.Error($"Database error during {nameof(SaveUserCredentialChanges)}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", ex);
                return false;
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Unexpected error during {nameof(SaveUserCredentialChanges)}. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}", ex);
                throw;

            }
        }

    }
}