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
        private static Lazy<Logger> _lazy = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _lazy.Value;

        private static string _pathToLog;
        public static void SetPathToLogger(string pathTolog)
        {
            _pathToLog = pathTolog;
        }

        private Logger()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                path: _pathToLog, 
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] TID:#{ThreadId}> {Message:lj} {NewLine}{Exception}")
                .Enrich.WithThreadId()
                .CreateLogger();
        }

        public void Info(string message) => Log.Information(message);
        public void Error(string message) => Log.Error(message);
        public void Warn(string message) => Log.Warning(message);
    }
}
