using Newtonsoft.Json;
using PasswordManager.Core.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace PasswordManager.Core.Helpers
{
    public static class FileWorker
    {
        public static int DefaultRandomSize { get; private set; } = 20;

        /// <summary>
        /// Write file to my documents
        /// </summary>
        /// <param name="account">Account, that contains list of instances AccountData</param>
        /// <returns></returns>
        public static bool WriteFile(Account account)
        {
            try
            {
                string forFile = JsonConvert.SerializeObject(account);
                int keyEncrypt = new Random().Next(1, DefaultRandomSize);
                string encryptedForFile = Encryption.Process(forFile, keyEncrypt);

                using (BinaryWriter bw = new BinaryWriter(File.Open(Constants.PathToMainFile, FileMode.Create)))
                {
                    bw.Write(keyEncrypt);
                    bw.Write(encryptedForFile);
                }
                return true;
            }
            catch
            {
                MessageBox.Show("Problems while write file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Read file data
        /// </summary>
        /// <returns></returns>
        public static async Task<Account> ReadFileAsync()
        {
            try
            {
                int keyDecrypt;
                string encryptedFromFile;
                string fromFile;

                using (var fileStream = File.Open(Constants.PathToMainFile, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fileStream))
                    {
                        keyDecrypt = br.ReadInt32();
                        encryptedFromFile = br.ReadString();
                    }
                }

                fromFile = Encryption.Process(encryptedFromFile, keyDecrypt);
                Account account = new Account();
                account = JsonConvert.DeserializeObject<Account>(fromFile);
                return account;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("New file will be created", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            catch
            {
                MessageBox.Show("File is corrupt, new file will be created instead of it", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                File.Create(Constants.PathToMainFile);
                return null;
            }
        }
    }
}
