using Pri.LongPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core
{
    public static class Constants
    {
        private static readonly string _appName = "PasswordManager";
        private static readonly string _nameOfPasswordsFile = "PassMan.dat";
        private static readonly string _nameOfLoggerFile = "PasswordManager.log";

        public static string LocalAppDataDirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName);
        public static string RoamingAppDataDirectoryPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appName);
        public static string PathToPasswordsFile => Path.Combine(RoamingAppDataDirectoryPath, _nameOfPasswordsFile);
        public static string PathToLoggerFile => Path.Combine(LocalAppDataDirectoryPath, _nameOfLoggerFile);
    }
}
