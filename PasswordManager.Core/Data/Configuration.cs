using Newtonsoft.Json;
using PasswordManager.Core.Helpers;
using PasswordManager;
using Pri.LongPath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PasswordManager.Core.Encryption;
using PasswordManager.Core.Threading;
using PasswordManager.Core.Extensions;
using System.Security.Cryptography;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace PasswordManager.Core.Data
{
    public class Configuration
    {
        #region Singleton
        private static Lazy<Configuration> _lazy = new Lazy<Configuration>(() => null);
        public static Configuration Instance => _lazy.Value;
        #endregion

        #region Properties
        public static bool InstanceInitialized { get; private set; }
        public Account CurrentAccount { get; set; } = new Account();
        #endregion

        #region Constructors
        private Configuration()
        {
            ReloadAccount();
            Logger.Info("Successfully read data file!");
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initialize new <see cref="Configuration"/> instance.
        /// </summary>
        public static bool InitializeConfiguration()
        {
            if (InstanceInitialized)
                return true;

            Logger.Info("Start reading configuration...");

            _lazy = new Lazy<Configuration>(() => new Configuration());
            var instance = Instance;

            if (Instance.CurrentAccount is null)
                return false;

            InstanceInitialized = true;
            return true;
        }

        /// <summary>
        /// Save account data.
        /// </summary>
        /// <param name="saveReason">Reason why should save</param>
        /// <returns></returns>
        public bool SaveAccount(string saveReason = null)
        {
            Logger.Info(saveReason is null ? 
                "Save data to file" : 
                $"Save data to file: \"{saveReason}\"");

            var dirPath = Pri.LongPath.Path.GetDirectoryName(Constants.PathToPasswordsFile);
            if (!Pri.LongPath.Directory.Exists(dirPath))
            {
                Pri.LongPath.Directory.CreateDirectory(dirPath);
            }

            try
            {
                string forFile = JsonConvert.SerializeObject(CurrentAccount);
                string forFileEncrypted = TripleDESHelper.EncryptString(forFile);

                var waitHandleName = Constants.PathToPasswordsFile.GetHashString<SHA256Managed>();
                using (var waitHandleLocker = EventWaitHandleLocker.MakeWithEventHandle(true, EventResetMode.AutoReset, waitHandleName))
                {
                    using (var fileStream = Pri.LongPath.File.Open(Constants.PathToPasswordsFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.Write(forFileEncrypted);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.HandleException(ex);
                return false;
            }
        }

        /// <summary>
        /// Read file with account data.
        /// </summary>
        /// <returns><see cref="Account"/> instance.</returns>
        private void ReloadAccount()
        {
            if (Pri.LongPath.File.Exists(Constants.PathToPasswordsFile))
            {
                try
                {
                    var waitHandleName = Constants.PathToPasswordsFile.GetHashString<SHA256Managed>();
                    using (var waitHandleLocker = EventWaitHandleLocker.MakeWithEventHandle(true, EventResetMode.AutoReset, waitHandleName))
                    {
                        using (var fileStream = Pri.LongPath.File.Open(Constants.PathToPasswordsFile, FileMode.Open))
                        {
                            using (StreamReader streamReader = new StreamReader(fileStream))
                            {
                                try
                                {
                                    var fromFileEncrypted = streamReader.ReadToEnd();
                                    var fromFileDecrypted = TripleDESHelper.DecryptString(fromFileEncrypted);
                                    CurrentAccount = JsonConvert.DeserializeObject<Account>(fromFileDecrypted);
                                }
                                catch (Exception ex)
                                {
                                    Logger.HandleException(ex);
                                    if (CurrentAccount is null)
                                        CurrentAccount = new Account();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }
        #endregion
    }
}
