using Password_Manager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Password_Manager
{
    class FileProcess
    {
        private readonly string PathToMainFile;
        private readonly int DefaultRandomSize;
        public FileProcess()
        {
            PathToMainFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\testdata.dat";
            DefaultRandomSize = 20;
        }
        /// <summary>
        /// Write file to my documents
        /// </summary>
        /// <param name="datas">List of instances AccountData</param>
        /// <param name="correctPass">Correct password</param>
        /// <returns></returns>
        public bool WriteFile(AccountData[] datas, string correctPass)
        {
            try
            {
                Encryption.KEY = new Random().Next(1, DefaultRandomSize);

                using (BinaryWriter bw = new BinaryWriter(File.Open(PathToMainFile, FileMode.Create)))
                {
                    //Header Ключ для шифрования и зашифрованный пароль
                    bw.Write(Encryption.KEY);
                    bw.Write(Encryption.Process(correctPass));

                    //Зашифрованные данные
                    foreach (AccountData data in datas)
                    {
                        bw.Write(Encryption.Process(data.Name));
                        bw.Write(Encryption.Process(data.Login));
                        bw.Write(Encryption.Process(data.Password));
                        bw.Write(Encryption.Process(data.Other));
                    }
                }
                return true;
            }
            catch
            {
                MessageBox.Show("File is corrupt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private long offsetToRead;

        /// <summary>
        /// Read file data
        /// </summary>
        /// <returns></returns>
        public AccountData[] ReadFile()
        {
            try
            {
                List<AccountData> accountDatas = new List<AccountData>();
                using (BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    br.BaseStream.Position = offsetToRead;
                    AccountData data;
                    while (br.PeekChar() != -1)
                    {
                        data = new AccountData
                        {
                            Name = br.ReadString(),
                            Login = br.ReadString(),
                            Password = br.ReadString(),
                            Other = br.ReadString()
                        };
                        accountDatas.Add(data);
                    }
                }
                return accountDatas.ToArray();
            }
            catch
            {
                MessageBox.Show("File is corrupt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        /// <summary>
        /// Read password
        /// </summary>
        /// <returns></returns>
        public string ReadPassword()
        {
            try
            {
                string pass;
                using (BinaryReader br = new BinaryReader(File.Open(PathToMainFile, FileMode.Open)))
                {
                    //Получение ключа для дешифрования
                    Encryption.KEY = br.ReadInt32();
                    //Чтение пароля
                    pass = Encryption.Process(br.ReadString());
                    //Сохранение смещения позиции
                    offsetToRead = br.BaseStream.Position;
                }
                return pass;
            }
            catch (FileNotFoundException)
            {
                File.Create(PathToMainFile);
                MessageBox.Show("New file have been created", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            catch
            {
                MessageBox.Show("Header of file is corrupt", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
