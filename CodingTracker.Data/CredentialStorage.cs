using System;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialStorage;
using CodingTracker.Data.CredentialServices;
using CodingTracker.Common.ICredentialServices;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.Loggers; 
using System.Collections.Generic;
using CodingTracker.Common.IApplicationLoggers;

namespace CodingTracker.Data.CredentialStorage
{
    public class CredentialStorage : ICredentialStorage
    {
        private readonly IApplicationLogger _appLogger; // Injected logger
        private readonly ICredentialService _credentialService;
        private readonly IDatabaseManager _databaseManager;

        public CredentialStorage(IApplicationLogger logger, ICredentialService credentialService, IDatabaseManager databaseManager)
        {
            _appLogger = logger;
            _credentialService = credentialService;
            _databaseManager = databaseManager;
        }

        public void AddCredentials(UserCredentialDTO credential)
        {
            string hashedPassword = HashPassword(credential.Password);
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = new SQLiteCommand(@"
                    INSERT INTO
                        UserCredentials
                    (
                        UserId,
                        Username,
                        PasswordHash
                    )
                    VALUES
                    (
                        @UserId,
                        @Username,
                        @PasswordHash
                    )"
                            , connection);

                command.Parameters.AddWithValue("@UserId", credential.UserId);
                command.Parameters.AddWithValue("@Username", credential.Username);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                try
                {
                    command.ExecuteNonQuery();
                    _appLogger.Info("Credentials added successfully for {Username}", credential.Username);
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error("Failed to add credentials for {Username}. Error: {ErrorMessage}", credential.Username, ex.Message);
                }
            });
        }

        public void UpdateCredentials(int userId, string newUsername, string newPassword)
        {
            UpdateUserName(userId, newUsername);
            UpdatePassword(userId, newPassword);
        }

        public void UpdateUserName(int userId, string newUserName)
        {
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = new SQLiteCommand(@"
                    UPDATE UserCredentials
                    SET Username = @Username
                    WHERE UserId = @UserId", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Username", newUserName);

                try
                {
                    command.ExecuteNonQuery();
                    _appLogger.Info("Username updated successfully for UserId {UserId}", userId);
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error("Failed to update username for UserId {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                }
            });
        }

        public void UpdatePassword(int userId, string newPassword)
        {
            string hashedPassword = HashPassword(newPassword);
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = new SQLiteCommand(@"
                    UPDATE UserCredentials
                    SET PasswordHash = @PasswordHash
                    WHERE UserId = @UserId", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                try
                {
                    command.ExecuteNonQuery();
                    _appLogger.Info("Password updated successfully for UserId {UserId}", userId);
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error("Failed to update password for UserId {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                }
            });
        }

        public void DeleteCredentials(int userId)
        {
            _databaseManager.ExecuteCRUD(connection =>
            {
                using var command = new SQLiteCommand(@"
                    DELETE FROM UserCredentials
                    WHERE UserId = @UserId", connection);

                command.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    command.ExecuteNonQuery();
                    _appLogger.Info("Credentials deleted successfully for UserId {UserId}", userId);
                }
                catch (SQLiteException ex)
                {
                    _appLogger.Error("Failed to delete credentials for UserId {UserId}. Error: {ErrorMessage}", userId, ex.Message);
                }
            });
        }



        public string HashPassword(string password)
        {
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
                    return builder.ToString();
                }
            }
            catch (ArgumentNullException ex)
            {
                _appLogger.Error("Password cannot be null.", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                _appLogger.Error("An unexpected error occurred while hashing the password.", ex);
                throw;
            }
            catch (Exception ex)
            {
                _appLogger.Error("An error occurred while processing your request.", ex);
                throw;
            }
        }


    }
}


