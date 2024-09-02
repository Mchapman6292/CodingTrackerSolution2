using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.IAuthtenticationServices;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Common.DataInterfaces.IUserCredentialRepository;
using CodingTracker.Common.IUtilityServices;
using System.Data.SQLite;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


// resetPassword, updatePassword, rememberUser 
namespace CodingTracker.Common.IAuthenticationServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICredentialManager _credentialManager;
        private readonly CodingSessionDTO _codingSessionDTO;
        private readonly IUserCredentialDTOManager _userCredentialDTOManager;
        private readonly IQueryBuilder _queryBuilder;
        private readonly INewDatabaseRead _newDatabaseRead;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly IUtilityService _utilityService;

        private int _currentUserId;
        public AuthenticationService(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialManager, IUserCredentialDTOManager userCredentialDTOManager, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead, IUserCredentialRepository userCredentialRepository, IUtilityService utilityService)
        {
            _databaseManager = databaseManager;
            _credentialManager = credentialManager;
            _appLogger = appLogger;
            _userCredentialDTOManager = userCredentialDTOManager;
            _queryBuilder = queryBuilder;
            _newDatabaseRead = newDatabaseRead;
            _userCredentialRepository = userCredentialRepository;
            _utilityService = utilityService;
        }


        public async Task<bool> AuthenticateLogin(string username, string password, Activity activity)
        {
            _appLogger.Info($"Starting {nameof(AuthenticateLogin)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}. ");
            try
            {
                UserCredential loginCredential = await _userCredentialRepository.GetCredentialByUsername(username, activity);

 

                var inputHash = _utilityService.HashPassword(password, activity);
                var storedHash = loginCredential.PasswordHash;

                bool isValid = inputHash.Equals(storedHash, StringComparison.Ordinal);

                if (!isValid)
                {
                    _appLogger.Info($" Error during {nameof(AuthenticateLogin)} inputHash and storedHash are the not the same. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                    return false;
                }

                    _appLogger.Info($"{nameof(AuthenticateLogin)} successful. TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.");
                    return true;
                   
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(AuthenticateLogin)} TraceId: {activity.TraceId}, ParentId: {activity.ParentId}.", ex);
                return false;
            }
        }



        







         public UserCredentialDTO GetUserDetails(string username)
        {
            UserCredentialDTO userCredential = null;

            _databaseManager.ExecuteDatabaseOperation(connection =>
            {
                var commandText = _queryBuilder.CreateCommandTextForUserCredentials(
                    new List<string> { "UserId", "Username", "PasswordHash", "LastLogin" }, username: username);

                using (var command = new SQLiteCommand(commandText, connection))
                {
                    _queryBuilder.SetCommandParametersForUserCredentials(command, username: username);

                    using var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        userCredential = new UserCredentialDTO
                        {
                            UserId = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            LastLogin = reader.IsDBNull(reader.GetOrdinal("LastLogin")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("LastLogin"))
                        };
                    }
                }
            }, nameof(GetUserDetails));

            return userCredential;
        }




        public void ResetPassword(string username, string newPassword)
        {
            using (var activity = new Activity(nameof(ResetPassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(ResetPassword)}. TraceID: {activity.TraceId}, Username: {username}");
                try
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    string hashedPassword = _credentialManager.HashPassword(newPassword);

                    _databaseManager.ExecuteCRUD(connection =>
                    {
                        using var command = connection.CreateCommand();
                        command.CommandText = @"
                            UPDATE 
                                UserCredentials
                            SET 
                                PasswordHash = @HashedPassword
                            WHERE
                                Username = @Username";
                        command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                        command.Parameters.AddWithValue("@Username", username);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            _appLogger.Warning($"No user found with username {username}. PasswordHash reset failed. TraceID: {activity.TraceId}");
                        }
                        else
                        {
                            _appLogger.Info($"PasswordHash for user {username} has been successfully reset. Rows affected: {rowsAffected}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                        }
                    });
                    stopwatch.Stop();
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error($"An error occurred during {nameof(ResetPassword)} for username {username}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                }
            }
        }



    }
}

