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
        private static Lazy<Configuration> _lazy = new Lazy<Configuration>(() => new Configuration());
        public static Configuration Instance => _lazy.Value;

        public Account CurrentAccount { get; set; }

        private Configuration()
        {
            Logger.Instance.Info("Init or create file with data...");
            CurrentAccount = InitOrCreateDataFile();
        }

        private int _defaultRandomSize = 20;

        /// <summary>
        /// Save all data
        /// </summary>
        /// <param name="account">Account, that contains list of instances AccountData</param>
        /// <returns></returns>
        public bool SaveData()
        {
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
                Logger.Instance.Error("Can't save data");
                return false;
            }
        }

        /// <summary>
        /// Read file data
        /// </summary>
        /// <returns></returns>
        private Account InitOrCreateDataFile()
        {
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
                Logger.Instance.Warn("No file exist. New file will be created");
                Pri.LongPath.File.Create(Constants.PathToMainFile);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.ToString());
            }
            return account;
        }
    }
}
