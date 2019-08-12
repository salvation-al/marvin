﻿using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Serilog;
using SharedEntities;

namespace Marvin_Ubuntu
{
    class Program
    {
        private static Marvin.Marvin _marvin;

        public static void Main(string[] args)
        {
            try
            {
                var appSettings = GetAppSettings();
                var logger = new LoggerConfiguration()
                                  .MinimumLevel.Information()
                                  .WriteTo.File(appSettings.LogPath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                                  .CreateLogger();

                _marvin = new Marvin.Marvin(appSettings, logger);
                _marvin.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"GoogleCron app failed with error: {ex.Message}");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        static AppSettings GetAppSettings()
        {
            var appSettingsFilePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/appsettings.json";

            if (!File.Exists(appSettingsFilePath))
            {
                throw new FileNotFoundException("appsettings.json was not found or is not accessible");
            }

            var appSettingsContent = File.ReadAllText(appSettingsFilePath);
            if (string.IsNullOrEmpty(appSettingsContent))
                throw new Exception("Appsettings file is empty");

            return JsonConvert.DeserializeObject<AppSettings>(appSettingsContent);
        }
    }
}
