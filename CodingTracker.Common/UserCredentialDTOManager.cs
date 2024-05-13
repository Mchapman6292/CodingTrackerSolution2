
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

        void SetUserCredentialUserId(int? userId);

        int ReturnCurrentUserId();
    }



    public class UserCredentialDTOManager : IUserCredentialDTOManager
    {
        public UserCredentialDTO _currentUserCredentialDTO = new UserCredentialDTO();

        private readonly IApplicationLogger _appLogger;
        private int _currentUserId;



        public UserCredentialDTOManager(IApplicationLogger appLogger)
        {
            _appLogger = appLogger;
        }


        public UserCredentialDTO CreateUserCredentialDTO(string username, string password)
        {
            using (var activity = new Activity(nameof(CreateUserCredentialDTO)))
            {
                _appLogger.Debug($"Starting {nameof(CreateUserCredentialDTO)}. TraceID: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    return new UserCredentialDTO
                    {
                        Username = username,
                        PasswordHash = password
                    };
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An error occurred during {nameof(CreateUserCredentialDTO)}. Error: {ex.Message}. TraceID: {activity.TraceId}, Elapsed time: {stopwatch.ElapsedMilliseconds}.", ex);
                    return null; 
                }
                finally
                {
                    stopwatch.Stop();
                    _appLogger.Info($"{nameof(CreateUserCredentialDTO)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                }
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
                    _appLogger.Info($"{nameof(SetCurrentUserCredential)} completed successfully. userId: {userCredentialDTO.UserId}, Username:{userCredentialDTO.Username}, PasswordHash: {userCredentialDTO.PasswordHash}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
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



        public void UpdateCurrentUserCredentialDTO(UserCredentialDTO updatedUserCredentialDTO)
        {
            using (var activity = new Activity(nameof(UpdateCurrentUserCredentialDTO)).Start())
            {
                _appLogger.Info($"Starting {nameof(UpdateCurrentUserCredentialDTO)}. TraceID: {activity.TraceId}");

                var updates = new List<(string Name, object Value)>();

                if (_currentUserCredentialDTO.UserId != updatedUserCredentialDTO.UserId)
                {
                    _currentUserCredentialDTO.UserId = updatedUserCredentialDTO.UserId;
                    updates.Add(("userId", updatedUserCredentialDTO.UserId));
                }
                if (_currentUserCredentialDTO.Username != updatedUserCredentialDTO.Username)
                {
                    _currentUserCredentialDTO.Username = updatedUserCredentialDTO.Username;
                    updates.Add(("Username", updatedUserCredentialDTO.Username));
                }
                if (_currentUserCredentialDTO.PasswordHash != updatedUserCredentialDTO.PasswordHash)
                {
                    _currentUserCredentialDTO.PasswordHash = updatedUserCredentialDTO.PasswordHash;
                    updates.Add(("PasswordHash", updatedUserCredentialDTO.PasswordHash));
                }
                if (_currentUserCredentialDTO.LastLogin != updatedUserCredentialDTO.LastLogin)
                {
                    _currentUserCredentialDTO.LastLogin = updatedUserCredentialDTO.LastLogin;
                    updates.Add(("LastLogin", updatedUserCredentialDTO.LastLogin));
                }

                foreach (var update in updates)
                {
                    _appLogger.Debug($"Updated {update.Name} to {update.Value}.");
                }

                _appLogger.Info($"Updated {nameof(UpdateCurrentUserCredentialDTO)} successfully. TraceID: {activity.TraceId}");
            }
        }


        public void SetUserCredentialUserId(int? userId)
        {
            using (var activity = new Activity(nameof(SetUserCredentialUserId)).Start())
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                _appLogger.Debug($"Starting {nameof(SetUserCredentialUserId)}. TraceID: {activity.TraceId}.");

                if (_currentUserId >0)
                {
                    _appLogger.Debug($"_currentUserId already has a value {_currentUserId}.");
                }
                if (_currentUserId == 0) 
                {
                    _currentUserId = (int)userId;

                    _appLogger.Debug($"_currentUserId updated to {_currentUserId}, Eapsed time : {stopwatch.ElapsedMilliseconds}, TraceID: {activity.TraceId}");
                }
            }
        }

        public int ReturnCurrentUserId()
        {
            using (var activity = new Activity(nameof(ReturnCurrentUserId)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ReturnCurrentUserId)}. TraceID: {activity.TraceId}.");

                if (_currentUserCredentialDTO.UserId == 0)
                {
                    _appLogger.Info($"CurrentUserId is {_currentUserCredentialDTO.UserId} (default for not created.");
                }
                else
                {
                    return _currentUserCredentialDTO.UserId;
                }
                return _currentUserCredentialDTO.UserId;
            }
        }
    }
}
