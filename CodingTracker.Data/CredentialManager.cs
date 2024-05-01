﻿using System;
using System.Text;
using System.Security.Cryptography;
using CodingTracker.Common.UserCredentialDTOs;
using CodingTracker.Common.ICredentialManagers;
using CodingTracker.Common.IDatabaseManagers;
using System.Data.SQLite;
using CodingTracker.Common.IApplicationLoggers;
using System.Diagnostics;
using System.Net;


// Pass DTO as parameter to methods that act on multiple properties
namespace CodingTracker.Data.CredentialManagers
{
    public class CredentialManager : ICredentialManager
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IDatabaseManager _databaseManager;

        public CredentialManager(IApplicationLogger applogger,  IDatabaseManager databaseManager)
        {
            _appLogger = applogger;
            _databaseManager = databaseManager;
        }

        public void CreateAccount(string username, string password)
        {
            using (var activity = new Activity(nameof(CreateAccount)).Start())
            {
                _appLogger.Debug($"Starting {nameof(CreateAccount)}. TraceID: {activity.TraceId}, Username: {username}");

                string hashedPassword = HashPassword(password);
                DateTime accountCreationDate = DateTime.UtcNow;
                _appLogger.Debug($"PasswordHash hashed for {username}. TraceID: {activity.TraceId}, AccountCreationDate: {accountCreationDate}");

                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                            INSERT INTO UserCredentials
                            (
                                Username,
                                PasswordHash
                            )
                            VALUES
                            (
                                @Username,
                                @PasswordHash
                            )", connection);

                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                    try
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        _appLogger.Debug($"Executing INSERT command for {username}. TraceID: {activity.TraceId}");

                        int affectedRows = command.ExecuteNonQuery();

                        stopwatch.Stop();
                        _appLogger.Info($"Credentials added successfully for {username}. Rows affected: {affectedRows}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        _appLogger.Error($"Failed to add credentials for {username}. SQLite error code: {ex.ErrorCode}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    }
                    catch (Exception ex)
                    {
                        _appLogger.Error($"An unexpected error occurred while adding credentials for {username}. Error: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    }
                });
            }
        }

        private bool checkifCredentialsExist()
        {
            throw new NotImplementedException();
        }


        public bool IsAccountCreatedSuccessfully(string username)
        {
            using (var activity = new Activity(nameof(IsAccountCreatedSuccessfully)).Start())
            {
                _appLogger.Debug($"Starting {nameof(IsAccountCreatedSuccessfully)}. TraceID: {activity.TraceId}, Username: {username}");

                Stopwatch stopwatch = Stopwatch.StartNew();

                bool isCreated = false;
                _databaseManager.ExecuteCRUD(connection =>
                {
                    using var command = new SQLiteCommand(@"
                SELECT EXISTS(SELECT 1 FROM UserCredentials WHERE Username = @Username)",
                        connection);

                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        isCreated = Convert.ToBoolean(command.ExecuteScalar());
                        stopwatch.Stop();
                        _appLogger.Info($"{nameof(IsAccountCreatedSuccessfully)} completed for {username}. Account exists: {isCreated}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}");
                    }
                    catch (SQLiteException ex)
                    {
                        stopwatch.Stop();
                        _appLogger.Error($"Failed to check if account was created for {username}. Error: {ex.Message}. Execution Time: {stopwatch.ElapsedMilliseconds}ms. TraceID: {activity.TraceId}", ex);
                    }
                });

                return isCreated;
            }
        }


       


        public string HashPassword(string password) 
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




        public UserCredentialDTO GetCredentialById(int userId)//needed?
        {
            throw new NotImplementedException(" not implemented.");
        }
    }
}


