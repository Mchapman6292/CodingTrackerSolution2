
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using CodingTracker.Common.ICredentialManagers;

namespace CodingTracker.Common.UserCredentialDTOManagers
{
    public interface IUserCredentialDTOManager
    {
        UserCredentialDTO CreateUserCredentialDTO(string username, string password);
        void SetCurrentUserCredential(UserCredentialDTO userCredentialDTO);
        UserCredentialDTO GetCurrentUserCredential();
        void UpdateCurrentUserCredentialDTO(UserCredentialDTO userCredentialDTO);
    }



    public class UserCredentialDTOManager : IUserCredentialDTOManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly ICredentialManager _credentialManager;
        private UserCredentialDTO _currentUserCredentialDTO { get; set; }


        public UserCredentialDTOManager(IApplicationLogger appLogger, ICredentialManager credentialManager)
        {
            _appLogger = appLogger;
            _credentialManager = credentialManager;
        }


        public UserCredentialDTO CreateUserCredentialDTO(string username, string password)
        {
            using (var activity = new Activity(nameof(CreateUserCredentialDTO)))
            {
                _appLogger.Debug($"Starting {nameof(CreateUserCredentialDTO)}. TraceID: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                UserCredentialDTO newCredential = null;
                try
                {

                    string hashedPassword = _credentialManager.HashPassword(password);

                    UserCredentialDTO userCredentialDTO = new UserCredentialDTO
                    {
                        Username = username,
                        PasswordHash = hashedPassword
                    };
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(CreateUserCredentialDTO)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}.", ex);
                }

                stopwatch.Stop();
                _appLogger.Info($"{nameof(CreateUserCredentialDTO)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                return newCredential;
            }
        }


        public void SetCurrentUserCredential(UserCredentialDTO userCredentialDTO)
        {
            using (var activity = new Activity(nameof(SetCurrentUserCredential)).Start())
            {
                _appLogger.Debug($"Starting {nameof(SetCurrentUserCredential)}. TraceId: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    if (userCredentialDTO == null)
                    {
                        throw new ArgumentNullException(nameof(userCredentialDTO), "UserCredentialDTO parameter cannot be null.");
                    }

                    _currentUserCredentialDTO = userCredentialDTO;
                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(SetCurrentUserCredential)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(SetCurrentUserCredential)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}ms.", ex);
                    throw;

                }
            }
        }

        public UserCredentialDTO GetCurrentUserCredential()
        {
            using (var activity = new Activity(nameof(GetCurrentUserCredential)).Start())
            {
                _appLogger.Debug($"Starting {nameof(GetCurrentUserCredential)}. TraceID: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    if (_currentUserCredentialDTO == null)
                    {
                        throw new InvalidOperationException("No current user credential is set.");
                    }

                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(GetCurrentUserCredential)} completed successfully. User: {_currentUserCredentialDTO.Username}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");

                    return _currentUserCredentialDTO;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(GetCurrentUserCredential)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}ms.", ex);
                    throw; 
                }
            }
        }


        public void UpdateCurrentUserCredentialDTO(UserCredentialDTO userCredentialDTO) 
        {
            throw new NotImplementedException();
        }
    }
}
