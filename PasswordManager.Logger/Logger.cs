using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public static class Logger
    {
        internal static string PathToLogger => Pri.LongPath.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameOfLoggerFile);
        private static readonly string nameOfLoggerFile = @"PasswordManager\PasswordManager.log";

        static Logger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(PathToLogger, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void Info(string message) => Log.Information(message);
        public static void Error(string message) => Log.Error(message);
        public static void Warn(string message) => Log.Warning(message);
    }
}
