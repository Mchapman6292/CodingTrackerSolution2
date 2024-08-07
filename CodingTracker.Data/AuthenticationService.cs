﻿using CodingTracker.Common.CodingSessionDTOs;
using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using CodingTracker.Common.ILoginManagers;
using CodingTracker.Common.INewDatabaseReads;
using CodingTracker.Common.IQueryBuilders;
using CodingTracker.Common.UserCredentialDTOManagers;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.UserCredentials;
using CodingTracker.Data.Interfaces.IUserCredentialRepository;
using CodingTracker.Data.Repositories.UserCredentialRepository;
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

        private int _currentUserId;
        public AuthenticationService(IApplicationLogger appLogger, IDatabaseManager databaseManager, ICredentialManager credentialManager, IUserCredentialDTOManager userCredentialDTOManager, IQueryBuilder queryBuilder, INewDatabaseRead newDatabaseRead, IUserCredentialRepository userCredentialRepository)
        {
            _databaseManager = databaseManager;
            _credentialManager = credentialManager;
            _appLogger = appLogger;
            _userCredentialDTOManager = userCredentialDTOManager;
            _queryBuilder = queryBuilder;
            _newDatabaseRead = newDatabaseRead;
            _userCredentialRepository = userCredentialRepository;
        }


        public async Task<bool> AuthenticateLogin(string username, string password, string traceId, string parentId)
        {
            _appLogger.Info($"Starting {nameof(AuthenticateLogin)} TraceId: {traceId}, ParentId: {parentId}. ");
            try
            {
                UserCredential loginCredential = await _userCredentialRepository.GetCredentialByUsername(username, traceId, parentId);

 

                var inputHash = HashPassword(password, traceId, parentId);
                var storedHash = loginCredential.PasswordHash;

                bool isValid = inputHash.Equals(storedHash, StringComparison.Ordinal);

                if (!isValid)
                {
                    _appLogger.Info($" Error during {nameof(AuthenticateLogin)} inputHash and storedHash are the not the same. TraceId: {traceId}, ParentId: {parentId}.");
                    return false;
                }

                    _appLogger.Info($"{nameof(AuthenticateLogin)} successful. TraceId: {traceId}, ParentId: {parentId}.");
                    return true;
                   
            }
            catch (Exception ex)
            {
                _appLogger.Error($"Error in {nameof(AuthenticateLogin)} TraceId: {traceId}, ParentId: {parentId}.", ex);
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



        public string HashPassword(string password, string traceId, string parentId)
        {
            using (var activity = new Activity(nameof(HashPassword)).Start())
            {
                _appLogger.Debug($"Starting {nameof(HashPassword)}, TraceId: {activity.TraceId}.");
                Stopwatch stopwatch = Stopwatch.StartNew();

                try
                {
                    using (SHA256 sha256Hash = SHA256.Create())
                    {
                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                        StringBuilder builder = new StringBuilder();
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            builder.Append(bytes[i].ToString("x2"));
                        }

                        stopwatch.Stop();
                        _appLogger.Info($"{nameof(HashPassword)} completed successfully. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}.");
                        return builder.ToString();
                    }
                }
                catch (ArgumentNullException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Password cannot be null. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (EncoderFallbackException ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"Encoding error while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _appLogger.Error($"An unexpected error occurred while hashing the password. Error: {ex.Message}. Elapsed time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    throw;
                }
            }
        }
    }
}
