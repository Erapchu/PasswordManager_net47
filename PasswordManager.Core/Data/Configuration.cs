using PasswordManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Core.Data
{
    public class Configuration
    {
        private static Lazy<Configuration> _lazy = new Lazy<Configuration>(() => new Configuration());
        public static Configuration Instance => _lazy.Value;

        public Configuration()
        {
            Task.Run(async () =>
            {
                Account = await FileWorker.ReadFileAsync();
            });
        }

        public Account Account { get; set; }
    }
}
