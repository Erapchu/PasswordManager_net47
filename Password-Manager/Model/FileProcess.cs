using Newtonsoft.Json;
using Password_Manager.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Password_Manager
{
    class FileProcess
    {

        private static Lazy<FileProcess> _fileProcess = new Lazy<FileProcess>(() => new FileProcess());
        public static FileProcess Instance { get { return _fileProcess.Value; } }

        public string PathToMainFile { get; private set; }
        public int DefaultRandomSize { get; private set; } = 20;
        public string NameOfMainFile { get; private set; } = @"\testdata.dat";
        public FileProcess()
        {
            PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + NameOfMainFile;
        }

        /// <summary>
        /// Write file to my documents
        /// </summary>
        /// <param name="account">Account, that contains list of instances AccountData</param>
        /// <returns></returns>
        public bool WriteFile(Account account)
        {
            try
            {
                string forFile = JsonConvert.SerializeObject(account);
                int keyEncrypt = new Random().Next(1, DefaultRandomSize);
                string encryptedForFile = Encryption.Process(forFile, keyEncrypt);

                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
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
        public Account ReadFile()
        {
            try
            {
                int keyDecrypt;
                string encryptedFromFile;
                string fromFile;

                using(BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    keyDecrypt = br.ReadInt32();
                    encryptedFromFile = br.ReadString();
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
                File.Create(PathToMainFile);
                return null;
            }
        }
    }
}
