using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class Logger
    {
        private static string _pathToLog;
        public static void InitLogger(string pathTolog)
        {
            _pathToLog = pathTolog;
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                path: _pathToLog,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] TID:#{ThreadId}> {Message:lj} {NewLine}{Exception}")
                .Enrich.WithThreadId()
                .CreateLogger();
        }

        public static void Info(string message) => Log.Information(message);
        public static void Error(string message) => Log.Error(message);
        public static void Warn(string message) => Log.Warning(message);
        public static void HandleException(Exception ex) => Log.Error(ex.ToString());
    }
}
