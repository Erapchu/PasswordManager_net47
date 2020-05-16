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
        public Account CurrentAccount { get; set; }
        #endregion

        #region Fields
        private int _defaultRandomSize = 20;
        #endregion

        #region Constructors
        private Configuration()
        {
            CurrentAccount = InitOrCreateDataFile();
            if (CurrentAccount is null)
                Logger.Instance.Warn("Current account is \"null\"!");
            else
                CheckAccountInstance();

            Logger.Instance.Info("Successfully read data file!");
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

            _lazy = new Lazy<Configuration>(() => new Configuration());
            var instance = Instance;

            if (Instance.CurrentAccount is null)
                return false;

            InstanceInitialized = true;
            return true;
        }

        /// <summary>
        /// Save all data
        /// </summary>
        /// <param name="account">Account, that contains list of instances AccountData</param>
        /// <returns></returns>
        public bool SaveData()
        {
            Logger.Instance.Info("Save data to file");
            try
            {
                string forFile = JsonConvert.SerializeObject(CurrentAccount);
                int keyEncrypt = new Random().Next(1, _defaultRandomSize);
                string encryptedForFile = Encryption.Process(forFile, keyEncrypt);

                using (BinaryWriter bw = new BinaryWriter(Pri.LongPath.File.Open(Constants.PathToMainFile, FileMode.Create)))
                {
                    bw.Write(keyEncrypt);
                    bw.Write(encryptedForFile);
                }
                return true;
            }
            catch
            {
                Logger.Instance.Error("Can't save data.");
                return false;
            }
        }

        /// <summary>
        /// Read file data
        /// </summary>
        /// <returns></returns>
        private Account InitOrCreateDataFile()
        {
            Logger.Instance.Info("Init file with data...");
            Account account = null;
            try
            {
                account = new Account();

                int keyDecrypt;
                string encryptedFromFile;
                string fromFile;

                using (var fileStream = Pri.LongPath.File.Open(Constants.PathToMainFile, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fileStream))
                    {
                        keyDecrypt = br.ReadInt32();
                        encryptedFromFile = br.ReadString();
                    }
                }

                fromFile = Encryption.Process(encryptedFromFile, keyDecrypt);
                account = JsonConvert.DeserializeObject<Account>(fromFile);
            }
            catch (FileNotFoundException)
            {
                Logger.Instance.Warn("No file exist. New file will be created.");
                Pri.LongPath.File.Create(Constants.PathToMainFile);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());
            }
            return account;
        }
        #endregion

        #region Private methods
        private void CheckAccountInstance()
        {
            if (string.IsNullOrWhiteSpace(CurrentAccount.CorrectPassword))
                Logger.Instance.Warn("Saved password is corrupted");
            if (CurrentAccount.Credentials is null)
                CurrentAccount.Credentials = new CredentialsCollection();
        }
        #endregion
    }
}
