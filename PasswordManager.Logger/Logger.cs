using Serilog;
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
                .WriteTo.File(_pathToLog, rollingInterval: RollingInterval.Month)
                .CreateLogger();
        }

        public void Info(string message) => Log.Information(message);
        public void Error(string message) => Log.Error(message);
        public void Warn(string message) => Log.Warning(message);
    }
}
