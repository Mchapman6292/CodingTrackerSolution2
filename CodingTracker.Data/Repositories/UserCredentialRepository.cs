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

        public UserCredentialRepository(EntityContext context, IApplicationLogger appLogger, IAuthenticationService authService ) : base(context)

        {
            _appLogger = appLogger;
            _authService = authService;
        }

        public async Task<UserCredential> GetUserCredentialForLogin(string username, string password, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(GetUserCredentialForLogin)} TraceId: {traceId}, ParentId: {parentId}.");
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
                _appLogger.Error($"Error in {nameof(GetUserCredentialForLogin)} for username: {username}. TraceId: {traceId}, ParentId: {parentId}.", ex);
                return null;
            }
        }
    }
}
