﻿using CodingTracker.Common.IApplicationLoggers;
using CodingTracker.Common.IStartConfigurations;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace CodingTracker.Data.Configurations
{
    public class StartConfiguration : IStartConfiguration
    {
        private readonly IApplicationLogger _appLogger;
        private readonly IConfiguration _configuration;
        public string ConnectionString { get; private set; }
        public string DatabasePath { get; private set; }

        public StartConfiguration(IApplicationLogger appLogger, IConfiguration configuration)
        {
            _appLogger = appLogger;
            _configuration = configuration;
            LoadConfiguration();
        }

        public void LoadConfiguration()
        {
            using (var activity = new Activity(nameof(LoadConfiguration)).Start()) 
            {
                _appLogger.Debug($"Starting {nameof(LoadConfiguration)}. TraceID: {activity.TraceId}");

                try
                {
                    _appLogger.Debug($"The raw connection string is: {_configuration["ConnectionStrings:ConnectionString"]}");
                    ConnectionString = _configuration.GetSection("DatabaseConfig:ConnectionString").Value;
                    DatabasePath = _configuration.GetSection("DatabaseConfig:DatabasePath").Value;

                    if (string.IsNullOrEmpty(ConnectionString))
                    {
                        _appLogger.Error($"Connection string configuration is missing. TraceID: {activity.TraceId}");
                        throw new InvalidOperationException("Connection string configuration is missing.");
                    }

                    if (string.IsNullOrEmpty(DatabasePath))
                    {
                        _appLogger.Error($"Database path configuration is missing. TraceID: {activity.TraceId}");
                        throw new InvalidOperationException("Database path configuration is missing.");
                    }

                    _appLogger.Info($"Configuration loaded successfully. TraceID: {activity.TraceId}");
                }
                catch (Exception ex)
                {
                    _appLogger.Error($"Error loading configuration: {ex.Message}. TraceID: {activity.TraceId}", ex);
                    throw new InvalidOperationException("Error loading configuration: " + ex.Message, ex);
                }
            }
        }
    }
}